# SasoMVVM

**SasoMVVM** ist eine leichtgewichtige C#-MVVM-Library, die **aot/native-freundlich** ist und sich sowohl für Desktop-Anwendungen als auch für Spiele eignet.  
Sie bietet eine kompakte, moderne MVVM-Struktur mit zusätzlichen Features für schnelle Entwicklung.

---

## Features

- **Leichtgewichtige Model-Sammlung**: Mitgelieferte Basisklassen wie `NotifyObject`
- **Zwei Command-Arten**:
  - `ActionCommand` und `TaskCommand`
- `CommandCondition`-Unterstützung  
- **Vermittlung zwischen View und ViewModel**: `Prompt` ähnlich wie `ReactiveUI.Interaction`  
- **Umfangreiche Notify-Objekte**: PropertyChanged, CollectionChanged, Undo/Redo, Validierung  
- **Selector-Unterstützung**: für ComboBox oder Listen  
- **Descriptor-Funktionalität**: Zum Beispiel Enum-Integration für ComboBox-Freundliche Namen  
- **Primitive Lokalisierung**: kulturabhängige Labels und Inhalte  

---

## Installation

1. Repository klonen:  
```bash
git clone https://github.com/DEIN_USERNAME/SasoMVVM.git