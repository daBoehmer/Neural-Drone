using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller _instance;
    public static Controller Instance{get{return _instance;}}
    public ComputeShader ComShad;
    
    private void Awake(){
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    void Start()
    {
        NeuralNetwork nn = new NeuralNetwork(new int[]{2, 2, 2});
        nn.SetInputs(new float[]{1, 2});
        float[] output = nn.GetOuputs();
        for(int layer=1; layer<nn.Values.Length; layer++){
            print($"Layer {layer}");
            for(int node=0; node<nn.Values[layer].Length; node++){
                print($"Node {node}");
                print(nn.Bias[layer][node]);
                for(int weight=0; weight<nn.Weights[layer][node].Length; weight++){
                    print(nn.Weights[layer][node][weight]);
                }
                print(nn.Values[layer][node]);
            }
        }
        print(output[0]);
        print(output[1]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
