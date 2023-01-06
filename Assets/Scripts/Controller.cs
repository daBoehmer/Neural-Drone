using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller Instance{get{return _instance;}}
    public ComputeShader ComShad;
    public Plane[] RoomComponents;
    public readonly int[] Structure={27, 10, 10, 10, 4};
    public int DroneAmount=100, ParentAmount=10;
    [HideInInspector]
    public int ChildAmount;
    [SerializeField]
    private GameObject[] RoomComponentsGameObjects;
    [SerializeField]
    private GameObject DronePrefab;
    
    private static Controller _instance;
    private Statistics Statistics;
    private NeuralNetwork[] Networks;
    private int DronesLeft;
    private Drone[] Drones;
    private const float OffspringMutationRate=0.02f;

    
    private void Awake(){
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        RoomComponents = new Plane[RoomComponentsGameObjects.Length];
        for (int i = 0; i < RoomComponentsGameObjects.Length; i++)
            RoomComponents[i] = new Plane(RoomComponentsGameObjects[i].transform.forward, RoomComponentsGameObjects[i].transform.position);
    }



    void Start()
    {
        ChildAmount = ((ParentAmount-1)*ParentAmount)/2;
        Statistics = new Statistics();
        Networks = new NeuralNetwork[DroneAmount];
        Drones = new Drone[DroneAmount];

        for(int i=0; i<DroneAmount; i++){
            Drones[i] = Instantiate<GameObject>(DronePrefab, Vector3.up*5, Quaternion.identity).GetComponent<Drone>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(DronesLeft>0) return;

        float[] FitnessArray = new float[DroneAmount];
        for(int i=0; i<DroneAmount; i++)
            FitnessArray[i] = Drones[i].GetFitness();
        int[] SurvivingNetworkIndices = Selection.Select(FitnessArray);
        NeuralNetwork[] SurvivingNetworks = new NeuralNetwork[SurvivingNetworkIndices.Length];

        for(int i=0; i<SurvivingNetworkIndices.Length; i++)
            SurvivingNetworks[i] = Drones[SurvivingNetworkIndices[i]].GetNeuralNetwork();
        for(int i=0; i<SurvivingNetworkIndices.Length; i++){
            float MutationRate = 1; // TODO FORMULA
            Drones[i].SetNeuralNetwork(new NeuralNetwork(
                Structure,
                Mutation.Mutate(SurvivingNetworks[i].GetWeights(), MutationRate),
                Mutation.Mutate(SurvivingNetworks[i].GetBias(), MutationRate)
            ));
        }
        int ChildIndex = SurvivingNetworks.Length;
        for(int i=0; i<ParentAmount; i++){
            float[] WeightsA = Drones[i].GetNeuralNetwork().GetWeights();
            float[] BiasA = Drones[i].GetNeuralNetwork().GetBias();
            for(int j=i+1; j<ParentAmount; j++)
                Drones[ChildIndex++].SetNeuralNetwork(new NeuralNetwork(
                    Structure,
                    Mutation.Mutate(CrossOver.CrossItOver(WeightsA, Drones[j].GetNeuralNetwork().GetWeights()), OffspringMutationRate),
                    Mutation.Mutate(CrossOver.CrossItOver(BiasA, Drones[j].GetNeuralNetwork().GetBias()), OffspringMutationRate)
                ));
        }
        for(int i=DroneAmount-ParentAmount-ChildAmount; i<DroneAmount; i++)
            Drones[i].SetNeuralNetwork(new NeuralNetwork(Structure));

        foreach(Drone drone in Drones)
            drone.gameObject.SetActive(true);
    }

    public void KillDrone(Drone drone, DeathCause cause){
        DronesLeft--;
        drone.Kill();
    }
}
