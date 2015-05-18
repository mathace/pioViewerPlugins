# pioViewerPlugins

This is a project containing plugins for PioViewer.
Plugins in this repository are bundled together with official PioViewer distribution and
are a good starting point for those willing to write their own plugins.

## Contents

Repository contains two projects. PioViewerApi is the set of interfaces and classes offered by PioViewer and PioViewerPlugins contains some example plugins.

## Setting up development environment

To start working on plugins you should download Visual Studio 2013 (free community edition is perfect) and open the PioViewerPlugins.sln file.
You should be able to succesfully build the project immediately.

## How to start

As a starting point I recommended to read through plugins in https://github.com/kuba97531/pioViewerPlugins/tree/master/PioViewerPlugins/Tutorial directory
starting with 1, 2 etc. There will come more plugins in the future.
To see how they work you should change the plugin classes to public, rebuild the project and copy PioViewerPlugins.dll to PioViewer distribution Plugins directory.
