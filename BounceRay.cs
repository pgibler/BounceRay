using System.Collections.Generic;
using UnityEngine;
 
// Provides the capability to cast a ray that bounces off of physical surfaces.
//
// Usage:
//   var position = new Vector2(..);
//   var direction = new Vector2(..);
//   var magnitude = 5;
//   var layerMask = 1;
//   var bounceRay = BounceRay.Cast(position, direction, magnitude, layerMask);
//   Debug.Log("Bounce Ray values", bounceRay.endPoints.Count(), bounceRay.contacts.Count(), bounceRay.finalDirection.ToString());
//
public class BounceRay
{
    // BounceRay result state.
 
    public List<Vector2> endPoints;
    public List<RaycastHit2D> contacts;
    public bool bounced;
    public Vector2 finalDirection;
 
    // Returns all contact points from a bouncing ray at the specified position and moving in the specified direction.
    public static BounceRay Cast(Vector2 position, Vector2 direction, float magnitude, int layerMask)
    {
        // Initialize the return data.
        BounceRay bounceRay = new BounceRay
        {
            contacts = new List<RaycastHit2D>(),
            endPoints = new List<Vector2>(),
            finalDirection = direction.normalized
        };
 
        // If there is magnitude left...
        if (magnitude > 0)
        {
            // Fire out initial vector.
            RaycastHit2D hit = Physics2D.Raycast(position, direction, magnitude, layerMask);
 
            // Calculate our bounce conditions.
            bool hitSucceeded = hit.collider != null && hit.distance > 0;
            bool magnitudeRemaining = hit.distance < magnitude;
 
            // Get the final position.
            Vector2 finalPosition = hitSucceeded ? hit.point : position + direction.normalized*magnitude;
 
            // Draw final position.
            Debug.DrawLine(position, finalPosition, Color.green);
 
            // If the bounce conditions are met, add another bounce.
            if (hitSucceeded && magnitudeRemaining)
            {
                // Add the contact and hit point of the raycast to the BounceRay.
                bounceRay.contacts.Add(hit);
                bounceRay.endPoints.Add(hit.point);
 
                // Reflect the hit.
                Vector2 reflection = Math.Reflect((hit.point - position).normalized, hit.normal);
 
                // Create the reflection vector
                Vector2 reflectionVector = reflection;
 
                // Bounce the ray.
                BounceRay bounce = Cast(
                    hit.point,
                    reflectionVector,
                    magnitude - hit.distance,
                    layerMask);
 
                // Include the bounce contacts and origins.
                bounceRay.contacts.AddRange(bounce.contacts);
                bounceRay.endPoints.AddRange(bounce.endPoints);
 
                // Set the final direction to what our BounceRay call returned.
                bounceRay.finalDirection = bounce.finalDirection;
 
                // We've bounced if we are adding more contact points and origins.
                bounceRay.bounced = true;
            }
            else
            {
                // Add the final position if there is no more magnitude left to cover.
                bounceRay.endPoints.Add(finalPosition);
                bounceRay.finalDirection = direction;
            }
        }
 
        // Return the current position & direction as final.
        return bounceRay;
    }
}