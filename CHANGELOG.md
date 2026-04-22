# Version History

## Version 1.1.2 - 2026-04-22
- [Improvement] Improved ViewModel resolution logic including type matching and inheritance handling
- [Improvement] Improved error handling and exception messages during factory resolution
- [Addition] Extended ViewModel creation API in `ViewModelFactoryRegistry` with missing instance creation methods
- [Addition] Extended `ModelViewModelFactory` to support creating ViewModels without requiring a mandatory model parameter  
- [Addition] Introduced additional interfaces to avoid breaking changes to `IViewModelFactory`, ensuring backward compatibility  

---

## Version 1.1.1 - 2026-04-21

- [Improvement] Minor fixes and stability improvements in ViewModelInspector
- [Addition] ViewModelFactoryContext introduced for optional factory context support

---

## Version 1.1.0 - 2026-04-18

- [Improvement] ActionCommand and TaskCommand improved (sync/async execution handling and stability enhancements)- [Improvement] `ICommandRule`: added `CanExecuteChanged` event support  
- [Improvement] AOT-friendly design improvements (reduced runtime dependencies, safer patterns) 
- [Feature] Command Dispatcher Layer introduced (optional centralized update propagation)  
- [Feature] ViewModelInspector system introduced (property inspection support for ViewModels)   

---

## Version 1.0.0 - 2026-04-10

- Initial release