# Версии DigiTFactory.Libraries

Проверка Chat: 2026-11-25. Целевая платформа остаётся `.NET 8`.

| Пакет | Версия | Источник |
|---|---:|---|
| `DigiTFactory.Libraries.SeedWorks` | `0.9.5` | nuget.org |
| `DigiTFactory.Libraries.CommandRepository.Postgres` | `0.4.0` | nuget.org |
| `DigiTFactory.Libraries.EventBus.InMemory` | `0.2.0` | nuget.org |
| `DigiTFactory.Libraries.ReadRepository.Postgres` | `0.2.2` | nuget.org |

Все используемые пакеты совпадают с последними версиями на nuget.org и в
`DigiTFactory.Libraries/master` на момент проверки.

`ReadRepository.Postgres 0.2.2` опубликован на nuget.org, поэтому локальная pinned-сборка
`0.2.0` из коммита `a87e884574d5ca7f1fc103414c6e999fb7aede3a` больше не используется.
Источник `LocalLibraries` в `src/nuget.config` оставлен для возможных будущих неопубликованных
версий, но текущее решение восстанавливает все пакеты DTF.Libs из nuget.org.

Неиспользуемый источник `github-ekstrem` удалён из `src/nuget.config`: для restore больше не
требуется `GITHUB_TOKEN`, а строгая сборка не превращает предупреждение авторизации GitHub
Packages в ошибку.
