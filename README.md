# SasoMVVM

**SasoMVVM** is a lightweight C# MVVM library that is **AOT / native-friendly** and suitable for both desktop applications and games.  
It provides a compact, modern MVVM structure with additional features for fast development.

---

## Features

- **Lightweight model collection**: Includes base classes such as `NotifyObject`
- Built-in **commands** such as `ActionCommand` and `TaskCommand`
- Command conditions support
- **View ↔ ViewModel communication**: `Prompt`, similar to `ReactiveUI.Interaction`
- **Rich notify objects**: PropertyChanged, CollectionChanged, Undo/Redo, validation support
- **Selector support**: for ComboBox or list scenarios
- **Descriptor functionality**: e.g. enum integration with user-friendly display names
- **Basic localization support**: culture-based labels and content

---

## 🧠 Purpose

SasoMVVM is designed to help developers:

* work with structured data more easily
* simplify mapping between data and view models
* reduce repetitive MVVM boilerplate
* speed up development of configuration-driven and data-driven applications

---

## 📦 Supported Formats

| Platform | Status        | Requirements             | NuGet |
|----------|--------------|--------------------------|--------|
| Core     | ✅ Available  | .NET 7.0                | [![NuGet](https://img.shields.io/nuget/v/Sachssoft.Sasopuls)](https://www.nuget.org/packages/Sachssoft.Sasopuls) |
| Avalonia | ✅ Available  | .NET 7.0, Avalonia 11.0 | [![NuGet](https://img.shields.io/nuget/v/Sachssoft.Sasopuls.Avalonia)](https://www.nuget.org/packages/Sachssoft.Sasopuls.Avalonia) |
| WPF      | 🚧 Planned    | —                        | — |

---

## 🚀 Roadmap

Planned improvements include:

- Extended format support (XML, YAML, TOML)
- Improved serialization features
- Potential full object processing engine
- Further simplification of developer workflows

---

## 📥 Installation

1. Clone the repository:

```bash
git clone https://github.com/DEIN_USERNAME/SasoMVVM.git