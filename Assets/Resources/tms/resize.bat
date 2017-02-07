@echo off
set size=%1
set work_dir="%cd%"

for /R %%i in ("*.png") DO (
    echo gdal_translate -outsize %1 %1 %%i %%~pi%%~ni_h.raw
)