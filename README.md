# Ultima2cForUmbraco

This is a simple code to get started with building e-commerce web-site based on Ultimate2c and Umbraco 7

Umbraco is used as a front-end to show the data taken from Ultimate2c via rest api (json)

![catalog](https://snag.gy/GPuznF.jpg)

![item](https://snag.gy/A3QJmv.jpg)

![basket](https://snag.gy/A8KbHy.jpg)

![login](https://snag.gy/3v4GJo.jpg)

![registration](https://snag.gy/YoaUi4.jpg)


How to start:

1. Prerequisites
  - Ultimate2c version 5.6.0.0 and upper (see http://www.ultimatebusinessware.ru/2c/download/)
  - fresh Umbraco 7.5 installation (with no starter kits installed!)
2. Setup
  1. install package Ultima2cForUmbraco into umbraco, complete full steps of installation 
  2. check web.config's Ultima.WebServiceURL setting for proper Ultimate2c WebServce URL and port!
3. You're done! Open your site and use it

Session loss

Session data (basket contents, personal data) can get quickly lost due to default IIS settings
(the session idle timeout and recycling).
It's better to adjust them from the start:

Go to IIS Manager
On Window's server it's in the server Dashboard -> IIS -> Tools ->(IIS) Manager
![IIS Manager](https://snag.gy/ozbTjh.jpg)

On non-server windows' Run "inetmgr"

Expand the list at your left 
Select DefaultAppPool (if you haven't earlier selected a custom appPool for you site) -> RightClick ->Advanced Settings
![DefaultAppPool](https://snag.gy/hIywle.jpg)

Enhance Processor Model -> Idle Time-out (minutes)

![Idle](https://snag.gy/P1pqV4.jpg)

Down to Recycling
It's unrecommended to completely turn off recycling as it may potentially consume all server resources.
Your go is to limit the memory limit, after which the pool is recycled

![Recycling](https://snag.gy/wAFZ0g.jpg)
