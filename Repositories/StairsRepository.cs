using FireEscape.DBContext;
using FireEscape.Factories;

namespace FireEscape.Repositories;

public class StairsRepository(SqliteContext context, StairsFactory factory)
    : BaseObjectRepository<Stairs, Protocol>(context, factory), IStairsRepository;
