@echo off

echo   _____  __  __     _____                      _                 _           
echo  ^|  __ \^|  \/  ^|   ^|  __ \                    ^| ^|               ^| ^|          
echo  ^| ^|  ^| ^| \  / ^|   ^| ^|  ^| ^| _____      ___ __ ^| ^| ___   __ _  __^| ^| ___ _ __ 
echo  ^| ^|  ^| ^| ^|\/^| ^|   ^| ^|  ^| ^|/ _ \ \ /\ / / '_ \^| ^|/ _ \ / _` ^|/ _` ^|/ _ \ '__^|
echo  ^| ^|__^| ^| ^|  ^| ^|   ^| ^|__^| ^| (_) \ V  V /^| ^| ^| ^| ^| (_) ^| (_^| ^| (_^| ^|  __/ ^|   
echo  ^|_____/^|_^|  ^|_^|   ^|_____/ \___/ \_/\_/ ^|_^| ^|_^|_^|\___/ \__,_^|\__,_^|\___^|_^|   
echo                                                    by ���߂���

echo [1���Ԃ�DM�̉摜�̃_�E�����[�h]
echo �_�E�����[�h�J�n...

for /f "usebackq delims=" %%A in (`powershell -command "(Get-date).ToString(\"yyyy-MM-dd-HH-mm-ss\")"`) do set MY_DATE_TODAY=%%A
for /f "usebackq delims=" %%A in (`powershell -command "(Get-date).AddDays(-1).ToString(\"yyyy/MM/dd HH:mm:ss\")"`) do set MY_DATE=%%A
echo %MY_DATE% �ȍ~��DM���擾���܂�
echo.

call "%~dp0netcoreapp3.1\DiscordChatExporter.Cli.exe" exportdm --auto-token --after "%MY_DATE%" --media --reuse-media --only-image --format "PlainText" -o "%~dp0�_�E�����[�h�ꏊ\%MY_DATE_TODAY%"

pause