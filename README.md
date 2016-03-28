# Ultima2cForUmbraco

This is a simple code to get started with building e-commerce web-site based on Ultima2c and Umbraco 7

Umbraco is used as a front-end to show the data taken from Ultima2c via rest api (json)

Put this code into App_Code folder and then you can use UltimaWebService and SessionBasket classes and their methods

More details available at http://feeleen.ru/blog/beta-version-of-a-simple-shop-ultima2c-with-umbraco-7/

![catalog](http://feeleen.ru/media/1019/catalog.png?width=500&height=372.35449735449737)

![item](http://feeleen.ru/media/1026/good.png?width=500&height=377.38246505717916)

![basket](http://feeleen.ru/media/1021/basket.png?width=500&height=465.25323910482916)


How to start:

1. Prerequisites
  - Ultima2c (see ultima2c.com)
  - fresh Umbraco installation (with no starter kits installed!)
2. Setup
  1. install package Ultima2cForUmbraco into umbraco 
  2. republish entire site
  3. Go to App_Code folder of Umbraco ad do the following:
    - Change goodPhotoNodeID value in method GetGoodPhoto in GoodPhotosController.cs to real Id's of GoodPhoto node from umbraco Content section;
    - Change goodNodeID value in method GetGood in GoodsController.cs to real Id's of Good node from umbraco Content section;
3. You're done! Open your site and use it

