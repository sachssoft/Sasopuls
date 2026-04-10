namespace Sachssoft.Sasopuls
{
    public enum NullStoreBehavior
    {
        // Ignoriert den fehlenden Store, keine Änderung, keine Benachrichtigung.
        Ignore,

        // Löst eine Ausnahme aus.
        ThrowException,

        // Fehlt der Store, aber PropertyChanged und MarkDirty werden trotzdem ausgelöst.
        NotifyOnly
    }
}
