#!/bin/sh
set -eu
chown -R app:app /storage /keys
exec runuser -u app -- dotnet Project.Web.dll
