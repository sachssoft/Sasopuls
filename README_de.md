# Sachssoft Sasopuls

**Sachssoft Sasopuls** ist eine leichtgewichtige C#-MVVM-Library, die **AOT-/Native-freundlich** ist und sich sowohl für Desktop-Anwendungen als auch für Spiele eignet.  
Sie bietet eine kompakte, moderne MVVM-Struktur mit zusätzlichen Features für eine schnelle Entwicklung.

---

## Features

- **Leichtgewichtige Model-Basisklassen**: z. B. `NotifyObject`
- Mitgelieferte **Commands** wie `ActionCommand` und `TaskCommand`
- `ICommandRule` ist ein Interface zur Definition einer Bedingung, die entscheidet, ob ein Command ausgeführt werden darf.
- **Kommunikation zwischen View und ViewModel**: `Prompt`, ähnlich zu `ReactiveUI.Interaction`
- **Umfangreiche Notify-Objekte**: Unterstützung für `PropertyChanged` und `CollectionChanged`
- Erweiterte **Dirty-Tracking-Unterstützung**: z. B. Abfrage beim Schließen, ob ungespeicherte Änderungen vorhanden sind
- **Selector-Unterstützung**: z. B. für ComboBoxen oder Listen
- **Descriptor-Funktionalität**: z. B. für Enum-Integration mit UI-freundlichen Namen
- **Primitive Lokalisierung**: kulturabhängige Labels und Inhalte

---

## 🧠 Zielsetzung

Sachssoft Sasopuls soll Entwicklern helfen:

- strukturierte Daten einfacher zu verarbeiten
- Mapping zwischen Daten und ViewModel-Strukturen zu vereinfachen
- wiederkehrenden Boilerplate-Code zu reduzieren
- schneller mit UI- und Konfigurationslogik zu arbeiten

---

## 📦 Unterstützte Plattformen

| Plattform | Status         | Voraussetzungen | NuGet |
|-----------|----------------|----------------|--------|
| Basis     | ✅ Verfügbar    | .NET 7.0       | [![NuGet](https://img.shields.io/nuget/v/Sachssoft.Sasopuls)](https://www.nuget.org/packages/Sachssoft.Sasopuls) |
| Avalonia  | ✅ Verfügbar    | .NET 7.0, Avalonia 11.0 | [![NuGet](https://img.shields.io/nuget/v/Sachssoft.Sasopuls.Avalonia)](https://www.nuget.org/packages/Sachssoft.Sasopuls.Avalonia) |
| WPF       | 🚧 In Planung   | –              | – |

---

## 🚀 Perspektive

Geplant sind unter anderem:

- Implementierung eines Undo-/Redo-Systems
- Change-Tracking (weiterführend)
- zusätzliche häufig verwendete Models und ViewModels
- Fokus auf Performance, insbesondere für Game-UI-Szenarien
- weitere Plattformen in Planung

---

## Installation

1. Repository klonen:

```bash
git clone https://github.com/DEIN_USERNAME/Sachssoft.Sasopuls.git