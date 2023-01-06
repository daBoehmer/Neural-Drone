using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    //[SerializeField]
    public Transform[] Rotors;

    private NeuralNetwork Network;

    private float[] CurrentPowers;
    private const float k=9.303374113f, G=0.99f, A=0.01f; // rechnerisch ermittelt, Modell: log. Wachstum 
    private const float MaxPower = 20f, MaxTorque = 200f;
    private const int RotorAmount=4, DegPerSec=45000; // Etwas mehr bei 8000rpm
    private Rigidbody rb;
    private float Fitness;

    public void SetNeuralNetwork(NeuralNetwork network){this.Network=network;}
    public NeuralNetwork GetNeuralNetwork(){return this.Network;}

    public float GetFitness(){return Fitness;}

    float ClampPower(float x){
        return (A*G)/(A+(G-A)*Mathf.Exp(-G*k*x));
    }

    float TorqueSum(float[] Powers){
        return (Powers[0]+Powers[3]-1*(Powers[2]+Powers[3])) / 2;
    }

    void Awake(){
        rb = GetComponent<Rigidbody>();
        Network = new NeuralNetwork(Controller.Instance.Structure);
    }

    void Start()
    {
        CurrentPowers = new float[]{0, 0, 0, 0};
    }

    void ApplyForces(float[] Powers){
        float Torque = TorqueSum(Powers); // Torque between -1 and 1
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y+Torque*MaxTorque*Time.deltaTime, transform.localEulerAngles.z);

        for(int i=0; i<RotorAmount; i++){
            rb.AddForceAtPosition(Vector3.up * Powers[i]*MaxPower, Rotors[i].position, ForceMode.Force);
            Rotors[i].localEulerAngles = new Vector3(Rotors[i].localEulerAngles.x, Rotors[i].localEulerAngles.y+DegPerSec*Powers[i]*Time.deltaTime, Rotors[i].localEulerAngles.z);
        }

    }

    void FixedUpdate()
    {
        float[] Powers = Network.GetOutputs(new float[]{
            transform.position.x,
            transform.position.y,
            transform.position.z,
            transform.rotation.x,
            transform.rotation.y,
            transform.rotation.z,
            Mathf.Sin(transform.rotation.x),
            Mathf.Sin(transform.rotation.y),
            Mathf.Sin(transform.rotation.z),
            Mathf.Cos(transform.rotation.x),
            Mathf.Cos(transform.rotation.y),
            Mathf.Cos(transform.rotation.z),
            Mathf.Tan(transform.rotation.x),
            Mathf.Tan(transform.rotation.y),
            Mathf.Tan(transform.rotation.z),
            rb.velocity.x,
            rb.velocity.y,
            rb.velocity.z,
            rb.angularVelocity.x,
            rb.angularVelocity.y,
            rb.angularVelocity.z,
            Controller.Instance.RoomComponents[0].GetDistanceToPoint(transform.position), // To Do: Shader hat Ebenengleichungen für Wände (constant buffer) und berechnet die Distanzen auf der GPU
            Controller.Instance.RoomComponents[1].GetDistanceToPoint(transform.position),
            Controller.Instance.RoomComponents[2].GetDistanceToPoint(transform.position),
            Controller.Instance.RoomComponents[3].GetDistanceToPoint(transform.position),
            Controller.Instance.RoomComponents[4].GetDistanceToPoint(transform.position),
            Controller.Instance.RoomComponents[5].GetDistanceToPoint(transform.position)
        });
        ApplyForces(Powers);
    }

    public void Kill(){
        transform.position = Vector3.up*5;
        transform.localEulerAngles = Vector3.zero;
        gameObject.SetActive(false);
    }

}
