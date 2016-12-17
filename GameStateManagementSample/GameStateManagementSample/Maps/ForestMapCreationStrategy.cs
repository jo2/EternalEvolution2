using RogueSharp;
using RogueSharp.MapCreation;

namespace GameStateManagementSample.Maps {
    class ForestMapCreationStrategy<T> : BorderOnlyMapCreationStrategy<T> where T : IMap, new() {

        public ForestMapCreationStrategy(int width, int height) : base(width, height) {

        }

        public new T CreateMap() {
            T map = base.CreateMap();

            return map;
        }
    }
}
