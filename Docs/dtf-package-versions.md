# Версии DigiTFactory.Libraries

Обновление Chat: 2026-11-25. Целевая платформа остаётся `.NET 8`.

| Пакет | Версия | Источник |
|---|---:|---|
| `DigiTFactory.Libraries.SeedWorks` | `0.9.2` | nuget.org |
| `DigiTFactory.Libraries.CommandRepository.Postgres` | `0.4.0` | nuget.org |
| `DigiTFactory.Libraries.EventBus.InMemory` | `0.2.0` | nuget.org |
| `DigiTFactory.Libraries.ReadRepository.Postgres` | `0.2.0` | локальная сборка из `DigiTFactory.Libraries/master` |

`ReadRepository.Postgres 0.2.0` на момент обновления присутствует в `master`, но ещё не
опубликован на nuget.org (там последняя версия `0.1.2`). Пакет собран из pinned-коммита:

```
a87e884574d5ca7f1fc103414c6e999fb7aede3a
```

Команда воспроизводимой локальной сборки из корня checkout `DigiTFactory.Libraries`:

```
dotnet pack ReadRepository.Postgres/DigiTFactory.Libraries.ReadRepository.Postgres.csproj -c Release -o nupkg
```

Chat получает этот `.nupkg` через источник `LocalLibraries` из `src/nuget.config`.
После публикации `0.2.0` на nuget.org локальную сборку можно заменить опубликованным пакетом
без изменения версии в проектах.
