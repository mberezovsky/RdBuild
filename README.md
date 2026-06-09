# RdBuild

**RdBuild** — это распределенная система сборки для удаленного параллельного выполнения задач. Проект разработан по аналогии с Xoreax Incredibuild и предназначен для распределения вычислительной нагрузки между несколькими машинами в сети.

## Обзор

RdBuild позволяет распределять задачи сборки или вычислительные задачи между несколькими участниками (clients/servers), координируя их работу через центральный координатор. Система использует собственный бинарный протокол для обмена данными по TCP.

### Основные возможности
- Распределенное выполнение задач
- Многопоточная архитектура сервера
- TCP-сокеты для межпроцессного взаимодействия
- Бинарный протокол с секционной структурой
- Модульная система команд
- Координация серверов и управление задачами

## Архитектура

Система состоит из нескольких компонентов:

```
┌─────────────────┐
│  RdBuild.Client │ ──┐
└─────────────────┘   │
                      ├──> TCP Network ──> RdBuild.Coordinator ──> RdBuild.Server
┌─────────────────┐   │     (Socket)        (Job Management)      (Task Execution)
│  RdBuild.Server │ ──┘
└─────────────────┘
┌──────────────────────┐
│ RdBuild.Coordinator  │
└──────────────────────┘
┌───────────────────────────┐
│ RdBuild.Shared (Core)     │
│ - Protocol definitions    │
│ - Data structures         │
│ - Command system          │
└───────────────────────────┘
```

### Компоненты

#### RdBuild.Shared
Ядро системы, содержащее:
- Определения протокола (Request, Response, SectionHeader, Section)
- Базовые структуры данных
- Систему команд (CommandProcessor)
- Секционную структуру данных

#### RdBuild.Client
Клиентская часть, отвечающая за:
- Инициацию задач
- Отправку данных на серверы
- Обработку ответов

#### RdBuild.Server
Серверная часть, включающая:
- ServerDaemon — TCP-сервер, слушающий входящие соединения
- CommandServer — обработка входящих команд
- PackageProcessor — обработка пакетов данных
- Модули обработки команд

#### RdBuild.Coordinator
Центральный координатор, отвечающий за:
- Регистрацию и управление серверами
- Распределение задач между серверами
- Мониторинг состояния узлов
- Обработка запросов от клиентов

#### RdBuild.Shared.Tests
Набор unit-тестов для проверки:
- Парсинга запросов и ответов
- Обработки команд
- Секционной структуры данных
- Координатора

## Установка и сборка

### Требования
- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet/3.1) или выше
- Git (для клонирования репозитория)

### Сборка проекта

```bash
# Клонирование репозитория
git clone <repository-url>
cd RdBuild.master

# Восстановление зависимостей
dotnet restore

# Сборка решения
dotnet build

# Сборка в релизной конфигурации
dotnet build -c Release

# Запуск тестов
dotnet test RdBuild.Shared.Tests/RdBuild.Shared.Tests.csproj
```

### Публикация

```bash
# Публикация для Linux
dotnet publish -c Release -r linux-x64 --self-contained true

# Публикация для Windows
dotnet publish -c Release -r win-x64 --self-contained true

# Публикация для macOS
dotnet publish -c Release -r osx-x64 --self-contained true
```

## Структура проекта

```
RdBuild.master/
├── RdBuild.Client/              # Клиентская часть
│   └── Program.cs              # Точка входа клиента
│
├── RdBuild.Server/             # Серверная часть
│   ├── Program.cs              # Точка входа сервера
│   ├── ServerDaemon.cs         # TCP-сервер и обработка соединений
│   ├── CommandServer.cs        # Обработка команд
│   ├── PackageProcessor.cs     # Обработка пакетов (NotImplemented)
│   └── RequestReader.cs        # Чтение запросов (NotImplemented)
│
├── RdBuild.Coordinator/        # Центральный координатор
│   ├── Program.cs              # Точка входа координатора
│   ├── CoordinatorCommandsProcessor.cs  # Обработка команд координатора
│   └── ServerRegistry.cs       # Регистрация серверов
│
├── RdBuild.Shared/             # Общая библиотека
│   ├── Protocol/               # Определения протокола
│   │   ├── Request.cs          # Запросы (generic)
│   │   ├── Response.cs         # Ответы
│   │   ├── SectionHeader.cs    # Заголовки секций
│   │   └── Section.cs          # Секции данных
│   ├── Commands/               # Система команд
│   │   ├── CommandProcessor.cs # Базовый класс процессора команд
│   │   ├── CommandAttribute.cs # Атрибут команды
│   │   └── CommandResult.cs    # Результат команды
│   └── SectionHeaders/         # Специализированные заголовки
│
├── RdBuild.Shared.Tests/       # Тесты
│   ├── RequestTests.cs         # Тесты Request
│   ├── ResponseTests.cs        # Тесты Response
│   ├── ResourceProcessorTests.cs # Тесты обработки ресурсов
│   └── RoutesTests.cs          # Тесты маршрутов команд
│
├── .gitignore                  # Исключения для Git
├── RdBuild.sln                 # Решение Visual Studio
├── README.md                   # Документация
└── TODO.txt                    # План разработки
```

## Протокол

RdBuild использует бинарный протокол с секционной структурой:

```
┌─────────────────────────────────────────────────────────┐
│ Request/Response Header                                 │
├─────────────────────────────────────────────────────────┤
│ Section 1: Header (SectionHeader)                       │
├─────────────────────────────────────────────────────────┤
│ Section 2: Parameters (key-value pairs)                 │
├─────────────────────────────────────────────────────────┤
│ Section 3: Object (serialized data)                     │
├─────────────────────────────────────────────────────────┤
│ Section 4: File (binary data)                           │
└─────────────────────────────────────────────────────────┘
```

### Секции
- **Header** — метаданные запроса/ответа
- **Parameters** — параметры команды в формате ключ-значение
- **Object** — сериализованные данные (Newtonsoft.Json)
- **File** — бинарные данные файлов

## Ключевые классы

### Request<TCommandEnum>
Основной класс запроса, генерический по типу команды.

```csharp
public class Request<TCommandEnum> : Request
    where TCommandEnum : struct, Enum
{
    public Dictionary<string, string> Parameters { get; set; }
    public TObject Object { get; set; }
}
```

### Response
Класс ответа от сервера.

```csharp
public class Response
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}
```

### CommandProcessor<TECommandType>
Базовый класс для обработки команд с поддержкой атрибутов.

```csharp
public abstract class CommandProcessor<TECommandType>
    where TECommandType : struct, Enum
{
    protected readonly Dictionary<TECommandType, CommandHandler> _routes;
    
    public void RegisterRoute(TECommandType command, CommandHandler handler)
    public CommandResult Execute(TECommandType command, Request request)
}
```

### ServerDaemon
TCP-сервер, слушающий входящие соединения на указанном порту.

```csharp
public class ServerDaemon
{
    private TcpListener _listener;
    private readonly int _port;
    
    public void Start()
    public void Stop()
}
```

### CoordinatorCommandsProcessor
Обработка команд координатора, включая регистрацию серверов.

```csharp
public class CoordinatorCommandsProcessor : CommandProcessor<CoordinatorCommands>
{
    public void RegisterServer(string serverId, string endpoint)
    public void UnregisterServer(string serverId)
    public List<string> GetRegisteredServers()
}
```

## Тестирование

Проект использует NUnit для unit-тестов:

```bash
# Запуск всех тестов
dotnet test

# Запуск конкретного тестового класса
dotnet test --filter "FullyQualifiedName~ResourceProcessorTests"
```

### Тестовые классы
- **RequestTests** — проверка создания и парсинга запросов
- **ResponseTests** — проверка формирования ответов
- **ResourceProcessorTests** — тесты обработки ресурсов (атрибут [TestFixture])
- **RoutesTests** — тесты регистрации и выполнения маршрутов команд

## Зависимости

- **Newtonsoft.Json** (13.0.1) — сериализация объектов
- **.NET Core 3.1** — целевой фреймворк

## Известные ограничения

Некоторые компоненты еще не реализованы (см. TODO.txt):
- [`PackageProcessor`](RdBuild.Server/PackageProcessor.cs) — обработка пакетов данных
- [`RequestReader`](RdBuild.Server/RequestReader.cs) — чтение запросов из сетевого потока
- Модульная система сети
- Система управления задачами (job management)
- Консольный клиент

Предупреждения компилятора:
- CS0169 — некоторые поля не используются (marked with `[UsedImplicitly]`)

## Вклад

Вклад в проект приветствуется! Пожалуйста:
1. Форкните репозиторий
2. Создайте ветку для вашей функции (`git checkout -b feature/AmazingFeature`)
3. Зафиксируйте изменения (`git commit -m 'Add some AmazingFeature'`)
4. Отправьте в ветку (`git push origin feature/AmazingFeature`)
5. Откройте Pull Request

## Лицензия

Этот проект лицензирован по лицензии MIT. См. файл LICENSE для подробной информации.

## Благодарности

- Идея архитектуры вдохновлена [Xoreax Incredibuild](https://www.incredibuild.com/)
- Используется [Newtonsoft.Json](https://www.newtonsoft.com/json) для сериализации
