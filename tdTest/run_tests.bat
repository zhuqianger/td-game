@echo off
echo 正在运行塔防游戏网络模块测试...
echo.

cd /d "%~dp0"
dotnet test --verbosity normal

echo.
echo 测试完成！
pause
