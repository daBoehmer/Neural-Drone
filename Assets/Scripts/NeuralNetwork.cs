using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NeuralNetwork
{
    /*
    General Concept:
        Each Neural Network is an instance of this class. They are simple feedforward networks. Each NN has
            - 'm' input nodes 'Iₘ' (n is described by the variable "InputNodesAmount")
            - 'n' output nodes 'Oₙ' (m is described by the variable "OutputNodesAmount")
            - 'o' hidden Network 'Hₒ' (o is described by the variable "HiddenLayerAmount") with 'Hₒ,ₐ' nodes (neurons) and 'Hₒ
            - 'w' := m * H₁,ₐ + ... + Hₒ₋₁,ₐ * Hₒ,ₐ weights
    */
    public ComputeShader ComShad;
    private ComputeBuffer WeightsBuffer, ValuesBuffer, BiasBuffer;

    public float[][] Values; // (0) Layer; (1) Node
    public float[][][] Weights; // (0) Layer; (1) Node; (2) n-th Weight
    public float[][] Bias; // (0) Layer; (1) Node;
    private int WeightsAmount, ValuesAmount;

    private void Initialize(){
        System.Random Dice = new System.Random();
        for(int Layer=1; Layer<Values.Length; Layer++){
            for(int Node=0; Node<Values[Layer].Length; Node++){
                Values[Layer][Node] = 0;
                ValuesAmount++;
                Bias[Layer][Node] = (float)Dice.NextDouble()*2.5f-1.25f;
                for(int Weight=0; Weight<Values[Layer-1].Length; Weight++){
                    Weights[Layer][Node][Weight] = (float)Dice.NextDouble()*2.5f-1.25f;
                    WeightsAmount++;
                }
            }
        }
    }

    public NeuralNetwork(int[] Structure){
        Values = new float[Structure.Length][];
        Bias = new float[Structure.Length][];
        Weights = new float[Structure.Length][][];

        for(int Layer = 0; Layer<Structure.Length; Layer++){
            Values[Layer] = new float[Structure[Layer]];
            Bias[Layer] = new float[Structure[Layer]];
            Weights[Layer] = new float[Structure[Layer]][];
        }
        for(int Layer = 1; Layer<Structure.Length; Layer++){
            for(int Node=0; Node<Weights[Layer].Length; Node++){
                Weights[Layer][Node] = new float[Structure[Layer-1]];
            }
        }
        WeightsAmount = 0;
        ValuesAmount = 0;
        Initialize();
    }

    public NeuralNetwork(float[][] Values, float[][][] Weights, float[][] Bias){
        ComShad = Controller.Instance.ComShad;
        this.Values = Values;
        this.Weights = Weights;
        this.Bias = Bias; 
    }

    float DotProduct(float[] a, float[] b){
        if(a.Length != b.Length) throw new System.Exception("Dot Product can only be calculated if both vectors length are equal");
        float sum = 0;
        for(int i=0; i<a.Length; i++){
            sum += a[i] * b[i];
        }
        return sum;
    }

    float ActivationFunction(float x){
        return 1/(1+Mathf.Exp(-x));
    }

    public void SetInputs(float[] Inputs){
        if(Inputs.Length != Values[0].Length) throw new System.Exception("Anzahl der Inputs stimmt nicht mit Größe des Inputlayers überein");
        for(int Node=0; Node<Values[0].Length; Node++){
            Values[0][Node] = Inputs[Node];
        }
    }

    private void CalculateOutputs(){
        for(int Layer=1; Layer<Values.Length; Layer++){
            for(int Node=0; Node<Values[Layer].Length; Node++){
                Values[Layer][Node] = this.DotProduct(Values[Layer-1], Weights[Layer][Node]) + Bias[Layer][Node];
            }
        }
    }

    public float[] GetOuputs(){
        this.CalculateOutputs();
        float[] Outputs = new float[Values[Values.Length-1].Length];
        for(int Node=0; Node<Outputs.Length; Node++){
            Outputs[Node] = ActivationFunction(Values[Outputs.Length][Node]); // Nur bei den Outputnodes oder überall die activation function?
        }
        return Outputs;
    }

}
