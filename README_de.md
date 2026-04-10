# SasoMVVM

**SasoMVVM** ist eine leichtgewichtige C#-MVVM-Library, die **aot/native-freundlich** ist und sich sowohl für Desktop-Anwendungen als auch für Spiele eignet.  
Sie bietet eine kompakte, moderne MVVM-Struktur mit zusätzlichen Features für schnelle Entwicklung.

---

## Features

- **Leichtgewichtige Model-Sammlung**: Mitgelieferte Basisklassen wie `NotifyObject`
- Mitglieferte **Commands** wie `ActionCommand` und `TaskCommand`
- CommandCondition mit 
- **Vermittlung zwischen View und ViewModel**: `Prompt` ähnlich wie `ReactiveUI.Interaction`  
- **Umfangreiche Notify-Objekte**: PropertyChanged, CollectionChanged, Undo/Redo, Validierung  
- **Selector-Unterstützung**: für ComboBox oder Listen  
- **Descriptor-Funktionalität**: Zum Beispiel Enum-Integration für ComboBox-Freundliche Namen  
- **Primitive Lokalisierung**: kulturabhängige Labels und Inhalte  


---

## 🧠 Zielsetzung

Sasodoc soll Entwicklern helfen:

* strukturierte Daten einfacher zu verarbeiten
* Mapping zwischen Daten und Dokumentstrukturen zu vereinfachen
* wiederkehrenden Serialisierungsaufwand zu reduzieren
* schneller mit Konfigurations- und Datendateien zu arbeiten

---

## 📦 Unterstütze Formate

| Platform | Status         | Voraussetzungen | NuGet |
| ------ | -------------- | -------------------- | ----- |
| Basis   | ✅ Verfügbar    | .NET 7.0				   | [![NuGet](https://img.shields.io/nuget/v/Sachssoft.Sasopuls)](https://www.nuget.org/packages/Sachssoft.Sasopuls)      |
| Avalonia   | ✅ Verfügbar | .NET 7.0, Avalonia 11.0 | [![NuGet](https://img.shields.io/nuget/v/Sachssoft.Sasopuls.Avalonia)](https://www.nuget.org/packages/Sachssoft.Sasopuls.Avalonia)      |
| WPF    | 🚧 In Planung |			-			 | - |

---

## 🚀 Perspektive

Geplant sind unter anderem:

* Erweiterung der Formatunterstützung (XML, YAML, TOML)
* Verbesserte Serialisierungsfunktionen
* Mögliche vollständige Objektverarbeitung
* Weitere Vereinfachungen für Entwickler-Workflows

---

## Installation

1. Repository klonen:  
```bash
git clone https://github.com/DEIN_USERNAME/SasoMVVM.git