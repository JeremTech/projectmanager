# Project Manager
A software made to manage projects as graduation project.

Project Manager is split into two main things : Project Manager Core (this repository) and Modules.
In the project's team, I have developped Project Manager Core, Project Manager Module SDK and Project Manager Core Lib.
the others modules made to manage project are developped by others members of the project's team.

## Project Manager Core

This is the most important part of the project. He load all the modules, manage the inter-modules interactions, set-up (if needed) the remote database, allow user to create account on the database or log-in and manage connection settings to the database.

## Project Manager Module SDK

This part is used by module's creator. This solution has the base code and configuration needed to allow the module to work woth Project Manager Core.

## Project Manager Core Lib

This library allow to the core and modules to share the same codes. All interactions with the database, settings and utility things are in this library.


## Other infos

- All of the project has been made with Visual Studio Community 2019
- It use the .Net Framework
- All features are not finished, stabled or implemented
