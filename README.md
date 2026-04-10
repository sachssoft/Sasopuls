# Sachssoft Sasopuls

**Sachssoft Sasopuls** is a lightweight C# MVVM library that is **AOT-/Native-friendly** and suitable for both desktop applications and games.  
It provides a compact, modern MVVM structure with additional features designed for fast development.

---

## Features

- **Lightweight base model classes**: e.g. `NotifyObject`
- Built-in **commands** such as `ActionCommand` and `TaskCommand`
- `ICommandRule` is an interface that defines a condition determining whether a command is allowed to execute
- **Communication between View and ViewModel**: `Prompt`, similar to `ReactiveUI.Interaction`
- **Rich notification support**: `PropertyChanged` and `CollectionChanged`
- Extended **dirty tracking support**: e.g. prompting the user on close if there are unsaved changes
- **Selector support**: e.g. for ComboBoxes or list-based UI elements
- **Descriptor functionality**: e.g. Enum integration with UI-friendly display names
- **Basic localization support**: culture-dependent labels and content

---

## 🧠 Goals

Sachssoft Sasopuls aims to help developers:

- process structured data more easily
- simplify mapping between data and ViewModel structures
- reduce repetitive boilerplate code
- speed up UI and configuration logic development

---

## 📦 Supported Platforms

| Platform | Status | Requirements | NuGet |
|----------|--------|--------------|-------|
| Core     | ✅ Available | .NET 7.0 | [![NuGet](https://img.shields.io/nuget/v/Sachssoft.Sasopuls)](https://www.nuget.org/packages/Sachssoft.Sasopuls) |
| Avalonia  | ✅ Available | .NET 7.0, Avalonia 11.0 | [![NuGet](https://img.shields.io/nuget/v/Sachssoft.Sasopuls.Avalonia)](https://www.nuget.org/packages/Sachssoft.Sasopuls.Avalonia) |
| WPF       | 🚧 Planned | – | – |

---

## 🚀 Roadmap

Planned features include:

- Undo/Redo system implementation
- Extended change tracking
- Additional commonly used models and ViewModels
- Focus on performance, especially for game UI scenarios
- Additional platform support

---

## Installation

1. Clone the repository:

```bash
git clone https://github.com/sachssoft/Sachssoft.Sasopuls.git