using Backpack;
using Operator;
using Stage;

namespace Common
{
    public static class ModuleInit
    {
        public static void Init()
        {
            ModelInit();
            NetworkInit();
        }

        public static void ModelInit()
        {
            BackpackModel.Init();
            OperatorModel.Init();
            StageModel.Init();
        }

        public static void NetworkInit()
        {
            BackpackNetwork.Init();
            OperatorNetwork.Init();
            StageNetwork.Init();
        }
    }
}