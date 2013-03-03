@echo off

if not exist ThirdParty (
   mkdir ThirdParty
)

pushd ThirdParty

if not exist NUnit-2.6.2 (
   curl --location -O http://launchpad.net/nunitv2/2.6.2/2.6.2/+download/NUnit-2.6.2.zip
   unzip NUnit-2.6.2.zip
   del NUnit-2.6.2.zip
)

popd
