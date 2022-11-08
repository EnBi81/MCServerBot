set rootFolder=C:\Users\enbi8\source\repos\MCServerBot
set swaggerCodegenCli=%rootFolder%\Tools\gitignored\swagger-codegen-cli.jar
set swaggerJson=%rootFolder%\code\LogicApi\MCWebAPI\swagger.json


set csApiDir=%rootFolder%\code\Presentation\ApiCs
set csConfig=config-csharp.json
set csOutputDir=%csApiDir%

set jsApiDir=%rootFolder%\code\Presentation\ApiJs
set jsConfig=config-javascript.json
set jsOutputDir=%jsApiDir%


:: check if swagger cli is installed, if not then download it
if not exist %swaggerJson% ( Powershell.exe -executionpolicy remotesigned -command "Invoke-WebRequest -OutFile %swaggerCodegenCli% https://repo1.maven.org/maven2/io/swagger/codegen/v3/swagger-codegen-cli/3.0.20/swagger-codegen-cli-3.0.20.jar" 
)

:: call the generators
call :run_generator csharp, %csOutputDir%, %csConfig%
call :run_generator javascript, %jsOutputDir%, %jsConfig%

:: build csharp project
cd Presentation\ApiCs\src\MCClient\
dotnet build


echo Apis created
pause
exit


:run_generator
rmdir /S /Q %~2
java --add-opens=java.base/java.util=ALL-UNNAMED -jar %swaggerCodegenCli% generate -i %swaggerJson% -l %~1 -o %~2 -c %~3
EXIT /B 0