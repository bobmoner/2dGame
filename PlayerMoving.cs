using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;


namespace PlayerMove{
	public class PlayerMoving : MonoBehaviour {

	    public float maxSpeed = 10f;
		public float jumpForce = 700f;
		public LayerMask whatIsGround;
		public Transform groundCheck;
		public Rigidbody2D fireballRigidbody;
		public Transform fireballSpawn;

		//---------Player stats----------
		public static float hpMax = 100f;
		public static float hpCurrent;
		public static float manaMax = 50f;
		public static float manaCurrent;
		public static float manaRegen = 1f;
		public static float damage = 10f;
		public static float hpRegen = 2f;
		public static float criticalChance = 5f;
		public static float defence = 10f;
		public static float fireballSpeed = 200f;
		public static float fireballDamage = 30f;
		public static float fireballManacost = 15f;
		//-------------------------------
		private float tikRate = 1f;
		private float tik;



	    Rigidbody2D rb;
	    Animator anim;
	    bool grounded = false;
		public bool facingRight = true;
	    float groundRadius = 0.2f;

		public GameObject sword;

	    void Start () {
	        rb = GetComponent<Rigidbody2D>();
	        anim = GetComponent<Animator>();
			hpCurrent = hpMax;
			manaCurrent = manaMax;
		}
		
		void FixedUpdate () {
			Regeneration ();
			grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
	        anim.SetBool("Grounded", grounded);
	        anim.SetFloat("vSpeed", rb.velocity.y);

			float move = CrossPlatformInputManager.GetAxis ("Horizontal");
			anim.SetFloat ("Speed",Mathf.Abs(move));
			rb.velocity = new Vector2 (move * maxSpeed, rb.velocity.y);

	        if (move > 0 && !facingRight)
	            Flip();
			else if (move < 0 && facingRight)
	            Flip();
	    }

	    void Update()
	    {
			if (hpCurrent <= 0)
				PlayerDie ();
			
	        if(grounded && Input.GetKeyDown(KeyCode.Space))
	        {
	            anim.SetBool("Grounded", false);
	            rb.AddForce(new Vector2(0, jumpForce));
	        }
	    }

	    void Flip()
	    {
	        facingRight = !facingRight;
	        Vector3 theScale = transform.localScale;
	        theScale.x *= -1;
	        transform.localScale = theScale;
	    }

		void PlayerDie(){
			maxSpeed = 0f;
			anim.Play ("Death");
			Destroy (gameObject,0.5f);
		}

		public void Jump(){
			if (grounded) {
				rb.AddForce (new Vector2 (0, jumpForce));
			}
		}

		public void Attack(){
			anim.Play ("Attack"); 
		}

		public void Cast(){
			if (manaCurrent - fireballManacost > 0) {
				manaCurrent -= fireballManacost;
				anim.Play ("Cast");
				Rigidbody2D fireball = Instantiate (fireballRigidbody, fireballSpawn.position, Quaternion.identity);
				fireball.AddForce (new Vector2 (facingRight ? fireballSpeed : -fireballSpeed, 0));
			}
		}

		public void enableSword(){
			sword.SetActive (true);
		}

		public void disableSword(){
			sword.SetActive (false);
		}

		void Regeneration(){
			if (tik > 0)
				tik -= Time.deltaTime;
			else {
				hpCurrent = hpCurrent + hpRegen;
				if (hpCurrent > hpMax)
					hpCurrent = hpMax;

				manaCurrent = manaCurrent + manaRegen;
				if (manaCurrent > manaMax)
					manaCurrent = manaMax;

				tik = tikRate;
			}
		}
			
	}
}
