# Autofac 101

## Prerequisites
- Visual Studio

## Introduction
Welcome to Autofac 101!  This 20-30 minute mini-hack will guide you through basic principles of Autofac.  To get started, clone this repository and load the Autofac101 solution.  If you stumble along the way, create an issue.

## Getting Started
Autofac is a simple, yet powerful dependency container.  Autofac assists us in implementing patters like [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection) and [IoC](https://en.wikipedia.org/wiki/Inversion_of_control).  The goal of this course is not to cover those topics, but to get your feet wet with what Autofac can do for you.

## Setup
After cloning the repository, you should find yourself looking at a simple console application that doesn't even compile.  Let's fix that.

Install the latest stable version of [Autofac](https://www.nuget.org/packages/Autofac/).  Our application bootstrapper needs to set up our Autofac registrations before we can start the app.  With Autofac, everything generally starts with a ```ContainerBuilder``` so let's create one in our ```Bootstrap``` function.
```
public Application Bootstrap()
{
	var builder = new ContainerBuilder();
}
```
We can now use this builder to create registrations.  Let's register our depdendencies.