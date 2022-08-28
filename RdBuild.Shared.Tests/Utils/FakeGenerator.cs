using Bogus;

namespace RdBuild.Shared.Tests.Utils
{
    class FakeGenerator
    {
        private Bogus.Faker m_faker = new Faker();

        public string GenerateString()
        {
            return m_faker.Random.String(null, 'A', 'я');
        }

        public int GenerateInt()
        {
            return m_faker.Random.Int();
        }

        public static FakeGenerator Instance { get; } = new FakeGenerator();
    }
}