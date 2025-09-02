using Backpack;
using Operator;
using Player;
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
            PlayerModel.Init();
            BackpackModel.Init();
            OperatorModel.Init();
            StageModel.Init();
            
        }

        public static void NetworkInit()
        {
            PlayerNetwork.Init();
            BackpackNetwork.Init();
            OperatorNetwork.Init();
            StageNetwork.Init();
            
        }
    }
}