terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
    }
  }
  required_version = ">= 1.3.7"
  backend "s3" {
    bucket                  = "terraform-buckets-food-totem"
    key                     = "food-totem-demand/terraform.tfstate"
    region                  = "us-west-2"
  }
}
provider "aws" {
    region = var.lab_account_region
}

data "aws_security_group" "default" {
  name = "default"
}

data "aws_vpc" "default" {
  default = true
}

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
}

resource "aws_lb_target_group" "demand-mongodb-tg" {
  name     = "demand-mongodb"
  port     = 27017
  protocol = "TCP"
  vpc_id   = data.aws_vpc.default.id
  target_type = "ip"
}

resource "aws_lb" "demand-mongodb-lb" {
  name               = "demand-mongodb"
  internal           = true
  load_balancer_type = "network"
  subnets            = data.aws_subnets.default.ids
}

resource "aws_lb_listener" "demand-mongodb-lbl" {
  load_balancer_arn = aws_lb.demand-mongodb-lb.arn
  port              = 27017
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.demand-mongodb-tg.arn
  }
}

resource "aws_lb_target_group" "demand-api-tg" {
  name     = "demand-api"
  port     = 8080
  protocol = "HTTP"
  vpc_id   = data.aws_vpc.default.id
  target_type = "ip"

  health_check {
    enabled = true
    interval = 300
    path = "/health-check"
    protocol = "HTTP"
    timeout = 60
    healthy_threshold = 3
    unhealthy_threshold = 3
  }
}

resource "aws_lb" "demand-api-lb" {
  name               = "demand-api"
  internal           = true
  load_balancer_type = "application"
  subnets            = data.aws_subnets.default.ids
}

resource "aws_lb_listener" "demand-api-lbl" {
  load_balancer_arn = aws_lb.demand-api-lb.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.demand-api-tg.arn
  }
}

resource "aws_ecs_task_definition" "food-totem-demand-mongodb-task" {
  family                   = "food-totem-demand-mongodb"
  network_mode             = "awsvpc"
  execution_role_arn       = "arn:aws:iam::${var.lab_account_id}:role/LabRole"
  cpu                      = 256
  memory                   = 512
  requires_compatibilities = ["FARGATE"]
  container_definitions    = jsonencode([
    {
        "name": "food-totem-demand-mongodb",
        "image": var.mongo_image,
        "essential": true,
        "portMappings": [
            {
              "containerPort": 27017,
              "hostPort": 27017,
              "protocol": "tcp"
            }
        ],
        "environment": [
            {
                "name": "MONGO_INITDB_ROOT_USERNAME",
                "value": var.mongo_root_user
            },
            {
                "name": "MONGO_INITDB_ROOT_PASSWORD",
                "value": var.mongo_root_password
            }
        ],
        "cpu": 256,
        "memory": 512,
        "logConfiguration": {
            "logDriver": "awslogs",
            "options": {
                "awslogs-group": "food-totem-demand-mongodb-logs",
                "awslogs-region": var.lab_account_region,
                "awslogs-stream-prefix": "food-totem-demand-mongodb"
            }
        }
    }
  ])
}

resource "aws_ecs_task_definition" "food-totem-demand-task" {
  depends_on = [ aws_ecs_task_definition.food-totem-demand-mongodb-task ]
  family                   = "food-totem-demand"
  network_mode             = "awsvpc"
  execution_role_arn       = "arn:aws:iam::${var.lab_account_id}:role/LabRole"
  cpu                      = 256
  memory                   = 512
  requires_compatibilities = ["FARGATE"]
  container_definitions    = jsonencode([
    {
        "name": "food-totem-demand",
        "image": var.food_totem_demand_image,
        "essential": true,
        "portMappings": [
            {
              "containerPort": 8080,
              "hostPort": 8080,
              "protocol": "tcp"
            }
        ],
        "environment": [
            {
                "name": "DemandDatabaseSettings__ConnectionString",
                "value": join("", ["mongodb://", var.mongo_root_user, ":", var.mongo_root_password, "@", aws_lb.demand-mongodb-lb.dns_name, ":27017"])
            }
        ],
        "cpu": 256,
        "memory": 512,
        "logConfiguration": {
            "logDriver": "awslogs",
            "options": {
                "awslogs-group": "food-totem-demand-logs",
                "awslogs-region": var.lab_account_region,
                "awslogs-stream-prefix": "food-totem-demand"
            }
        }
    }
  ])
}

resource "aws_ecs_service" "food-totem-demand-mongodb-service" {
  name            = "food-totem-demand-mongodb-service"
  cluster         = "food-totem-ecs"
  task_definition = aws_ecs_task_definition.food-totem-demand-mongodb-task.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    security_groups  = [data.aws_security_group.default.id]
    subnets = data.aws_subnets.default.ids
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.demand-mongodb-tg.arn
    container_name   = "food-totem-demand-mongodb"
    container_port   = 27017
  }

  health_check_grace_period_seconds = 120
}

resource "aws_ecs_service" "food-totem-demand-service" {
  name            = "food-totem-demand-service"
  cluster         = "food-totem-ecs"
  task_definition = aws_ecs_task_definition.food-totem-demand-task.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    security_groups  = [data.aws_security_group.default.id]
    subnets = data.aws_subnets.default.ids
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.demand-api-tg.arn
    container_name   = "food-totem-demand"
    container_port   = 8080
  }

  health_check_grace_period_seconds = 120
}

resource "aws_cloudwatch_log_group" "food-totem-demand-logs" {
  name = "food-totem-demand-logs"
  retention_in_days = 1
}

resource "aws_cloudwatch_log_group" "food-totem-demand-mongodb-logs" {
  name = "food-totem-demand-mongodb-logs"
  retention_in_days = 1
}

