# Repositório Genérico Utilizando Dapper

Este é um projeto .NET 8.0 que implementa o repositório genérico utilizando o Dapper, assim como outros conceitos, como serviço genérico, controlador base, injeções de dependência e Unit of Work.

## Visão Geral

O padrão repositório é de uso popular para o acesso a dados em aplicações .NET. Esse projeto leva o conceito de repositório mais além, implementando o Dapper de forma genérica, viabilizando a criação de operações CRUD (create, read, update, delete), proporcionando métodos padrões de forma fácil e rápida, com somente algumas configurações.

O intuito desse projeto é suprir a necessidade da utilização de um repositório genérico em aplicações .NET onde a implementação do EF Core acaba se tornando inviável.

Obs: Nomes de classes, métodos,  descrições e comentários em inglês para abranger mais pessoas.

## Como Utilizar

A arquitetura já está toda preparada para ser utilizada, desde a configuração com o banco de dados até os 'endpoints' do controlador, necessitando apenas de alguns ajustes e configurações.

### Configurando a Conexão com Banco de Dados

A primeira coisa a se fazer é configurar a 'string' de conexão com o banco de dados, substituindo o valor da variável `connectionString`, que se encontra na classe `DbConnectionConfig`. 
```csharp
public static IServiceCollection ResolveDbConnection(this IServiceCollection services)
{
    string connectionString = "connectionString";
```

### Adicionando a Classe Modelo

A classe modelo se refere aos objetos que serão manipulados nas camadas de regra de negócio da aplicação.

O próximo passo é adicionar uma nova classe na pasta `Models` da camada `Domain` e faça com que ela herde da classe base `BaseModel`. É aconselhável que suas propriedades possuam nomes mais amigáveis, para facilitar no entendimento e nas atribuições de seus respectivos valores.
```csharp
public class Model : BaseModel
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
}
```

### Adicionando a Classe de Entidade

A classe de entidade se refere aos objetos que serão manipulados na camada de comunicação com o banco de dados.

Agora é a vez da classe de entidade ser criada na pasta `Entities` da mesma camada `Domain`,  onde além dela herdar da classe base `BaseEntity`, ela deverá conter uma anotação com o nome da tabela da qual essa classe representa. Suas propriedades devem possuir os mesmos nomes contidos nos campos da respectiva tabela.
```csharp
[Table("TABLE_NAME")]
public class Entity : BaseEntity
{
    public int ID { get; set; }
    public string? DESCRIPTION { get; set; }
    public DateTime DATE { get; set; }
}
```

### Mapeamento entre as classes

Para que a camada de conexão com o banco de dados saiba a relação entre as propriedades das classes de modelo e de entidades, devemos configurar o mapeamento entre as classes, com a ajuda do 'AutoMapper'.

A configuração é feita na classe `AutomapperConfig` na pasta `Config` da camada `Domain`.
```csharp
public AutomapperConfig()
{
    CreateMap<Entity, Model>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DESCRIPTION))
        .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DATE))
        .ReverseMap();
}
```

### Criando o Repositório

Para que os métodos das operações padrões do CRUD estejam disponíveis para as camadas de regra de negócio, uma classe concreta do repositório deve ser criada, assim como sua interface.

Comece criando a interface do repositório na pasta `Repositories\Intf` da camada `Infrastructure`,  herdando da interface base `IBaseRepository<TModel, TEntity>`, informando os respectivos tipos das classes modelo e de entidade criadas anteriormente.
```csharp
public interface IRepository : IBaseRepository<Model, Entity>
{
}
```

Com a interface criada, agora é a vez de criar a classe concreta do repositório na pasta `Repositories\Impl` da mesma camada `Infrastructure`, implementando sua interface e herdando a classe base `BaseRepository<TModel, TEntity>`, informando os respectivos tipos das classes modelo e de entidade criadas anteriormente, além de realizar as injeções das dependências em seu construtor para serem passadas ao construtor base.
```csharp
public class Repository : BaseRepository<Model, Entity>, IRepository
{
    public Repository(SqlConnection connection, IDbTransaction transaction, IMapper mapper) : base(connection, transaction, mapper)
    {
    }
}
```

O próximo passo é adicionar o serviço do repositório na classe de configuração de injeção de dependências `DependencyInjectionConfig`, passando o tipo da interface e da classe criadas anteriormente.
```csharp
public static IServiceCollection ResolveDependencies(this IServiceCollection services)
{
    services.AddScoped<IRepository, Repository>();
```

Com o repositório adicionado na injeção de dependências, chegou a vez de incluí-lo na classe `UnitOfWork`, adicionando uma nova propriedade para o repositório recém criado e injetando sua respectiva instância no controlador.
```csharp
public class UnitOfWork : IUnitOfWork
{
    public IDbTransaction _transaction;
    public IRepository Repository { get; }

    public UnitOfWork(IDbTransaction transaction, IRepository repository)
    {
        _transaction = transaction;
        Repository = repository;
    }
```

Com esses passos, as camadas de regra de negócio podem acessar o repositório a partir do `UnitOfWork`, adicionando uma propriedade e injetando sua instância pelo construtor.
```csharp
public class Service : BaseService<Model, Entity, IRepository>, IService
{
    private readonly IUnitOfWork _uow;

    public Service(IUnitOfWork uow) : base(uow, uow.Repository)
    {
        _uow = uow;
    }
}
```

Com isso, basta utilizar a propriedade recém criada `_uow` para selecionar o repositório e realizar a operação desejada.
```csharp
public async Task<Model> GetAsync(int id)
{
    return await _uow.Repository.GetAsync(e => e.ID == id);
}
```

Lembrando que ao realizar operações que necessitem de persistência no banco de dados, deve-se utilizar o método `Commit()` presente no `UnitOfWork`, para que a atual transação persista os dados modificados.
```csharp
public async Task<int> UpdateAsync(Model model, int id)
{
    int response = await _uow.Repository.UpdateAsync(model, e => e.ID == id);
    _uow.Commit();
    return response;
}
```

## Utilizações Opcionais

Além das implementações demonstradas anteriormente, este projeto está configurado para proporcionar mais funcionalidades, a partir do serviço genérico e do controlador genérico.

### Utilizando o Serviço Genérico

O serviço genérico foi criado com o intuito de facilitar a implementação das operações CRUD, proporcionando ao controlador o acesso aos métodos padrões de um determinado repositório.

Obs: mesmo que um serviço herde do serviço genérico os métodos padrões de um determinado repositório, ele poderá utilizar métodos de outros repositórios registrados no `UnitOfWork`.

Para começar a utilizá-lo, crie a interface do serviço na pasta `Services\Intf` da camada `Application`,  herdando da interface base `IBaseService<TModel, TEntity>`, informando os tipos das classes modelo e de entidade relacionadas ao repositório escolhido para ser utilizado como principal pelo serviço.
```csharp
public interface IService : IBaseService<Model, Entity>
{
}
```

Com a interface criada, agora é a vez de criar a classe do serviço na pasta `Services\Impl` da mesma camada `Application`, implementando sua interface e herdando da classe base `BaseService<TModel, TEntity, TIRepository>`, informando os respectivos tipos da classe modelo,  da classe de entidade e da interface do repositório escolhido, além de adicionar uma propriedade para acessar o `UnitOfWork`, fazer sua injeção de dependência no construtor e passá-lo ao construtor base, junto com a instância do repositório selecionado como principal.
```csharp
public class Service : BaseService<Model, Entity, IRepository>, IService
{
    private readonly IUnitOfWork _uow;

    public Service(IUnitOfWork uow) : base(uow, uow.Repository)
    {
        _uow = uow;
    }
}
```

O próximo passo é adicionar o serviço na classe de configuração de injeção de dependências `DependencyInjectionConfig`, passando o tipo da interface e da classe do serviço criado.
```csharp
public static IServiceCollection ResolveDependencies(this IServiceCollection services)
{
    services.AddScoped<IService, Service>();
```

Com esses passos, os controladores, que se encontram na pasta `Controllers` da camada `DapperGenRep.API`,  podem acessar os métodos padrões  do serviço, adicionando uma propriedade e injetando sua instância pelo construtor.
```csharp
[ApiController]
[Route("api/[controller]")]
public class DapperGenRepController : ControllerBase
{
    private readonly IService _service;

    public DapperGenRepController(IService service)
    {
        _service = service;
    }
```

Com isso, basta utilizar a propriedade recém criada `_service` para selecionar o método e realizar a operação desejada.
```csharp
[HttpGet("{filter}")]
public async Task<ActionResult<Model>> Get(int filter)
{
    return Ok(await _service.GetAsync(e => e.ID == filter));
}
```

### Utilizando o Controlador Base

O controlador base foi criado com a finalidade de tratar a resposta dos 'endpoints', de acordo com os retornos dos métodos padrões do serviço.

Para utilizá-lo, basta fazer com que o controlador herde da classe base `BaseController` e chamar o método `HandleResponse()`, passando o retorno da operação em seu parâmetro, assim tratando cada resposta de acordo com a sobrecarga identificada.
```csharp
public class DapperGenRepController : BaseController
{
    private readonly IService _service;

    public DapperGenRepController(IService service)
    {
        _service = service;
    }
    
	[HttpGet("{filter}")]
	public async Task<ActionResult<Model>> Get(int filter)
	{
	    Model response = await _service.GetAsync(e => e.ID == filter);
	    return HandleResponse(response);
	}
}
```
