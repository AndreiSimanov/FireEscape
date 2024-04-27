using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;

namespace FireEscape.Repositories;

public class StairsRepository(SqliteContext context, IStairsFactory factory)
    : BaseObjectRepository<Stairs, Protocol>(context, factory), IStairsRepository;