using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : MonoBehaviour
{
    private Collider2D col;
    private SpriteRenderer sprite;
    [SerializeField] private Dragon dragon;
    [SerializeField] private Animation anim;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        col.enabled = false;
        sprite.enabled = false;
        anim.Play();
        dragon.StartEating();
    }
}
