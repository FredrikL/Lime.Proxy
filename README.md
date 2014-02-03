# Lime.Proxy


## What?

Proxy for Lime Pro WebService by lundalogik that allows clientside javascript to invoke stored procedures and query tables.

## Limitations
* Expected input is json and only return type is json
* *No security what so ever*
* All changes needs to be done with stored procedures

## Installation
* Modify App.config and set correct endpoints
* Run installutil on limeproxy.service.exe
* Run `netsh http add urlacl url=http://+:PORT/ user=Everyone`
* Start service
