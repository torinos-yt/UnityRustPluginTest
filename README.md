UnityRustPluginTest  
==========================

![img](https://i.imgur.com/RT8q5jN.gif)  
This repository included test project for written native plugin for Unity using rust, and comparison native(Rust) method and managed(C#) method for triangulate simple polygon by ear clipping.  

In order to do a pure speed comparison here, we call the triangulate process every frame and expect the vertex input to always appear in a `clockwise direction`.

How to use
--------------------------
The Unity Project `only contains for Windows binary`.  
If you want to try the plugin on other platforms, please refer to `build.sh` in the source code directory `Plugin/`, and build the plugin for each platform `--target`.

Require
--------------------------
- Unity 2021.2 or later