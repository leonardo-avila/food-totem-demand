sonar-scan:
	dotnet clean
	dotnet sonarscanner begin /k:"<SONAR_PROJECT>" /d:sonar.host.url="<SONAR_URL>" /d:sonar.token="<SONAR_PROJECT_TOKEN>" /d:sonar.cs.opencover.reportsPaths="**\TestResults\*\*.xml"
	dotnet build
	dotnet test --no-build --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
	dotnet sonarscanner end /d:sonar.token="<SONAR_PROJECT_TOKEN>"

test:
	dotnet clean
	dotnet build
	dotnet test --no-build --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

full-clean:
	find . -type d -name "bin" -o -name "obj" -o -name "TestResults" | xargs rm -rf
	dotnet clean