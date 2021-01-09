@ECHO OFF

pushd %~dp0

REM Command file for Sphinx documentation

if "%SPHINXBUILD%" == "" (
	echo Cannot find environment variable for the Sphinx Build.
	echo Setting to 'sphinx-build'
	set SPHINXBUILD=sphinx-build
)
set SOURCEDIR=.
set BUILDDIR=_build

if "%1" == "" goto help

%SPHINXBUILD% >NUL 2>NUL
if errorlevel 9009 (
	echo.
	echo.The 'sphinx-build' command was not found. Make sure you have Sphinx
	echo.installed, then set the SPHINXBUILD environment variable to point
	echo.to the full path of the 'sphinx-build' executable. Alternatively you
	echo.may add the Sphinx directory to PATH.
	echo.
	echo.If you don't have Sphinx installed, grab it from
	echo.http://sphinx-doc.org/
	exit /b 1
)

REM python /home/docs/checkouts/readthedocs.org/user_builds/stravaigextensionsconfigurationdiagnostics/envs/latest/bin/sphinx-build
REM -T -E -W --keep-going -b readthedocs -d _build/doctrees-readthedocs -D language=en . _build/html
echo %SPHINXBUILD% -T -E -W --keep-going -b %1 -d _build/doctrees-readthedocs -D language=en %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%
%SPHINXBUILD% -T -E -W --keep-going -b %1 -d _build/doctrees-readthedocs -D language=en %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%
if errorlevel 0 (
	echo.
	echo Done!
) else (
	echo.
	echo FAILED!
)
goto end

:help
%SPHINXBUILD% -M help %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%

:end
popd
