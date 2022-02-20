using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentScript : Agent{

    public bool checkIfRayMovedGoal = false;

    public bool checkIfRayHitsObject = false;
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
    }

    //helper method to see if goal gets hit from RayPerceptionSensor
    private void RayCastInfo(RayPerceptionSensorComponent3D rayComponent)
    {
        var rayOutputs = RayPerceptionSensor
                .Perceive(rayComponent.GetRayPerceptionInput())
                .RayOutputs;
 
        if (rayOutputs != null)
        {
            var lengthOfRayOutputs = RayPerceptionSensor
                    .Perceive(rayComponent.GetRayPerceptionInput())
                    .RayOutputs
                    .Length;

            for (int i = 0; i < lengthOfRayOutputs; i++)
            {
                GameObject goHit = rayOutputs[i].HitGameObject;
                if (goHit != null)
                {
                    //am anfang habe ich kein laser (vor erstem hit)
                    //wenn ein hit dann durchgehend laser
                    //ich muss checken wann er immer in die erste if rein geht
                    //bin davon ausgegangen, dass das immer nur dann passiert wenn ray auch wirklich trifft
                    //set bool true if object gets hit
                    checkIfRayHitsObject = true;
                    AddReward(1.0f);
                }
                //set bool for ray doesn't hit goal
                else
                {
                    checkIfRayHitsObject = false;
                }
            }
        }
    }

    //Agent observations which come with every (frame/updadte)
    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        RayCastInfo(GetComponent<RayPerceptionSensorComponent3D>());
        //EndEpisode gets in CollectObservations called recursively
        //EndEpisode();
    }

    //gets discrete or continous values for actions
    public override void OnActionReceived(ActionBuffers actions){
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        //wenn der Ray getroffen hat dann kommt es auch automatisch zu einem Treffer vom Laser und daher zum reward. nach jedem reward beginnt die Epsiode neu.
        if(checkIfRayMovedGoal == true){
            EndEpisode();
        }
    }

    //just for testing
    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    //hier erstelle ich meine methode für reward 
    //immer wenn checkIfRayHitsObject = true; dann bekommt agent +1 reward
    //vielleicht kann ich einen winzigen -0.001 negativen reward einbauen, sodass sich der agent überhaupt bewegt und nicht einfach nur still steht
    //vorallem das result belohnen 
    //rewards between -1 1
    //end episode
    //
    //AddReward(1.0f);
    //ich will mir ein example anschauen wie dort mit endspisode gearbeittet wird
    //creating prefab and than train on 20 arenas
    //training --> reading train docs for ml agents, configure train file for 1 mio steps
    //start training? --> behaviors/config.file
    //ich muss herausfinden, woran es liegt dass mein training nicht startet
    //add force klappt nicht -- fixen
    //keine bewegung --> ich kann einfach algo für bewegung mit hitbox vergleichen
    //der agent muss seine localPosition wissen um sich bewegen zu können mit inference

    //training:
    //beim training funktioniert add force nicht
    //ich muss ein endepisode einbauen EndEpisode();

    //ich darf kein EndeEpisode in meine collectobservations rein nehmen
    //ich kann eine public var erstellen welche bei positiven reward auf einen wert gesetztz wird und dann bei e
    //current problen on which I work is that I cannot call my EndEpisode in my collectobservations because i get a recursive function call. can i call endepisode in any way inside of collect obeservations? wenn nicht wo kann ich endepisode sonst aufrufen?
    //ein agent lern von seinen episoden wenn ich keine habe dann kann mein agetn auch nicht lernen
    //macht es überhaupt sinn den blauen agenten nicht zu respawn? weil ein spiel geht ja immer so lang bis der laser einmal getroffen hat.

    //wenn ich endepisode einsetze dann wird der agent zurückgesetzt bevor ich ihn durch laser weggeschossen habe
    //das kann ich umgehen wenn ich eine public variable setze wenn das goal weggeschossen wurde und wenn diese zb true ist kann ich die episode beenden
    //variable für den check ob laser goal wegbewegt hat (public var in )
    //mein objekt sinkt einfach nur in eine plattform ein (vllt wird bei episodebegin stable von seinem parent gelöst?)
    //ich glaube mein agent wird bei episode begin in der lust gespwaned und fliegt dann mit kraft (durch beschleunigung in der luft durch die plattform)
}

    



