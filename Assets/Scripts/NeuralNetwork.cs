 using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using UnityEngine;
public class NeuralNetwork
{
    [SerializeField]
    private ComputeShader GPU;
    private ComputeBuffer WeightsBuffer, InputsBuffer, BiasBuffer, OutputsBuffer,
        StructureBuffer, layerInputsBuffer, ValuesBuffer, PreviousValuesBuffer;

    private float[] Weights, Bias, Output;
    private int[] Structure;

    int maxElement(int[] a){
        int max=0;
        for(int i=0; i<a.Length; i++)
            if(a[i]>max)
                max = a[i];

        return max;
    }

    public float[] GetWeights(){return Weights;}
    public float[] GetBias(){return Bias;}

    private void initializeBuffers(){
        GPU = Controller.Instance.ComShad;

        Output = new float[Structure[Structure.Length-1]];

        WeightsBuffer = new ComputeBuffer(Weights.Length, sizeof(float));
        BiasBuffer = new ComputeBuffer(Bias.Length, sizeof(float));
        InputsBuffer = new ComputeBuffer(Structure[0], sizeof(float));
        StructureBuffer = new ComputeBuffer(Structure.Length, sizeof(int));
        OutputsBuffer = new ComputeBuffer(Structure[Structure.Length-1], sizeof(float));
        ValuesBuffer = new ComputeBuffer(maxElement(Structure), sizeof(float));
        PreviousValuesBuffer = new ComputeBuffer(maxElement(Structure), sizeof(float));

        GPU.SetBuffer(0, Shader.PropertyToID("weights"), WeightsBuffer);
        GPU.SetBuffer(0, Shader.PropertyToID("bias"), BiasBuffer);
        GPU.SetBuffer(0, Shader.PropertyToID("inputs"), InputsBuffer);
        GPU.SetBuffer(0, Shader.PropertyToID("structure"), StructureBuffer);
        GPU.SetBuffer(0, Shader.PropertyToID("outputs"), OutputsBuffer);
        GPU.SetBuffer(0, Shader.PropertyToID("valuesBuffer"), ValuesBuffer);
        GPU.SetBuffer(0, Shader.PropertyToID("previousBuffer"), PreviousValuesBuffer);

        WeightsBuffer.SetData(Weights);
        BiasBuffer.SetData(Bias);
        StructureBuffer.SetData(Structure);
        ValuesBuffer.SetData(new int[maxElement(Structure)]);
        PreviousValuesBuffer.SetData(new int[maxElement(Structure)]);

        GPU.SetInt(Shader.PropertyToID("w_l"), Weights.Length);
        GPU.SetInt(Shader.PropertyToID("i_l"), Structure[0]);
        GPU.SetInt(Shader.PropertyToID("b_l"), Bias.Length);
        GPU.SetInt(Shader.PropertyToID("s_l"), Structure.Length);
        GPU.SetInt(Shader.PropertyToID("o_l"), Structure[Structure.Length-1]);
    }
 
    public NeuralNetwork(int[] structure){ // Used for 1st generation drones
        System.Random Dice = new System.Random();
        // Generate Weights and Bias
        // Calculate amount of weights and bias
        ushort weightAmount = 0;
        ushort biasAmount = 0;
        for(byte Layer=1; Layer<structure.Length; Layer++){
            weightAmount += (ushort)(structure[Layer] * structure[Layer-1]);
            biasAmount += (ushort) structure[Layer];
        }

        // Initializing and filling Weights array
        Weights = new float[weightAmount];
        for(ushort weight=0; weight<weightAmount; weight++){
            Weights[weight] = (float)Dice.NextDouble()-0.5f; // Weights are initialized between -.5 and .5
        }

        // Initializing and filling Bias array
        Bias = new float[biasAmount];
        for(ushort biasIndex=0; biasIndex<biasAmount; biasIndex++)
            Bias[biasIndex] = (float)Dice.NextDouble()-0.5f;

        this.Output = new float[structure[structure.Length-1]];

        initializeBuffers();
    }

    public NeuralNetwork(int[] structure, float[] weights, float[] bias){ // Used for later generation drones
        this.Weights = weights;
        this.Bias = bias;
        this.Structure = structure;
        this.Output = new float[structure[structure.Length-1]];

        initializeBuffers();
    }

    public float[] GetOutputs(float[] inputs){
        InputsBuffer.SetData(inputs);
        GPU.Dispatch(GPU.FindKernel("ComputeOutputs"), 1, 1, 1);
        OutputsBuffer.GetData(Output);
        Kill();
        return Output;
    }

    public float[] CalculateOutputCPU(float[] input){
        return new float[10];
    }

    public void Kill(){
        WeightsBuffer.Release();
        BiasBuffer.Release();
        InputsBuffer.Release();
        OutputsBuffer.Release();
        StructureBuffer.Release();
        ValuesBuffer.Release();
        PreviousValuesBuffer.Release();
    }

}
 