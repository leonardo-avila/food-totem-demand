sonar-scan:
	dotnet clean
	dotnet sonarscanner begin /k:"food-totem-demand" /d:sonar.host.url="http://localhost:9000"  /d:sonar.token="sqp_13c50407883f135fc99cbaf35e7fb8def6527d4d" /d:sonar.cs.opencover.reportsPaths="**\TestResults\*\*.xml"
	dotnet build
	dotnet test --no-build --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
	dotnet sonarscanner end /d:sonar.token="sqp_13c50407883f135fc99cbaf35e7fb8def6527d4d"

test:
	dotnet clean
	dotnet build
	dotnet test --no-build --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

full-clean:
	find . -type d -name "bin" -o -name "obj" -o -name "TestResults" | xargs rm -rf
	dotnet clean

run-services:
	cd infra/local; docker-compose build --no-cache;
	cd infra/local; docker-compose up -d

run-database:
	cd infra/local;	docker-compose up -d demand-database

run-api:
	cd infra/local; docker-compose up -d demand-api

stop-services:
	cd infra/local; docker-compose down