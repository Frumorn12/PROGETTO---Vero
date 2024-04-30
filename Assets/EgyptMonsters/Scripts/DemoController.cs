using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DemoController : MonoBehaviour
{

	private Animator animator;

    
    [SerializeField] private Slider energyBar;
    [SerializeField] private Image fillImg;

	public float walkspeed = 5;
	private float horizontal;
	private float vertical;
	private float rotationDegreePerSecond = 1000;
	private bool isAttacking = false;

	public GameObject gamecam;
	public Vector3 camPosition;

    public Vector3 camRotation;

	private bool dead;


	public GameObject[] characters;
	public int currentChar = 0;

    public GameObject[] targets;
    public float minAttackDistance;

    public UnityEngine.UI.Text nameText;

    private bool direzione = false; // 5 false, -5 true

    public float jumpForce = 10f; // Forza base del salto
    public float additionalJumpForce = 5f; // Forza aggiuntiva del salto per ogni pressione del tasto "Tab"
    private float accumulatedJumpForce = 0f; // Forza accumulata del salto
    private bool isJumping = false; // Flag per indicare se il personaggio sta balzando

    public float transitionDuration = 0.5f;

    public bool scorrimento = false;

    private Vector3 originalCamPosition ;
    private Vector3 originalCamRotation ;

    bool timerStarted = false;



    public int energy; 
    // Start is called before the first frame update

    

public void tryDamageTarget1()
{
    target = null;
    float targetDistance = minAttackDistance + 1;
    foreach (var item in targets)
    {
        float itemDistance = (item.transform.position - transform.position).magnitude;
        if (itemDistance < minAttackDistance)
        {
            if (target == null) {
                target = item;
                targetDistance = itemDistance;
            }
            else if (itemDistance < targetDistance)
            {
                target = item;
                targetDistance = itemDistance;
            }
        }
    }
    
}

	void Start()
	{
		setCharacter(0);
        energy = 100; 
        
    }


    private void OnTriggerEnter(Collider other)
    {

        string objectTag = other.tag;
        Debug.Log("Il tag dell'oggetto che ha attivato il trigger è: " + objectTag);
        // Controlla se un oggetto è entrato nel trigger
        if (other.CompareTag("Scorrimento"))
        {
            // Attiva lo scorrimento o esegui altre azioni necessarie
            scorrimento = true;
        }
    }
     

	void FixedUpdate()
	{
		if (animator && !dead)
		{   
        
            
			//walk
			horizontal = Input.GetAxis("Horizontal"); // Questo fa in modo che il personaggio si muova in base all'input orizzontale, quindi a sinistra o a destra
            
			vertical = Input.GetAxis("Vertical"); // Questo fa in modo che il personaggio si muova in base all'input verticale, quindi in avanti o indietro


            
            // Assicurati che solo il tasto "W" possa muovere il giocatore in avanti

            if (direzione)
            {
                vertical = Input.GetAxis("Vertical"); // Inverti la direzione verticale
                horizontal = -Input.GetAxis("Horizontal"); // Inverti la direzione orizzontale
                vertical = Mathf.Clamp01(vertical);
                vertical= -vertical;
            }
            else
            {
                vertical = Input.GetAxis("Vertical");
                horizontal = Input.GetAxis("Horizontal");
                vertical = Mathf.Clamp01(vertical);
            }

            Vector3 stickDirection = new Vector3(horizontal, 0, vertical);
			
            float speedOut;

			if (stickDirection.sqrMagnitude > 1) stickDirection.Normalize();

			if (!isAttacking)
				speedOut = stickDirection.sqrMagnitude;
			else
				speedOut = 0;
             
            
			if (stickDirection != Vector3.zero && !isAttacking)
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(stickDirection, Vector3.up), rotationDegreePerSecond * Time.deltaTime);
            // Questo fa in modo che il personaggio si muova in base alla direzione in cui guarda il giocatore e alla velocità di camminata impostata in precedenza   
  
  			GetComponent<Rigidbody>().velocity = transform.forward * speedOut * walkspeed + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);

			animator.SetFloat("Speed", speedOut);
             // Controllo del volo
            if (isJumping)
            {
                // Applica la forza accumulata del salto verso l'alto
                GetComponent<Rigidbody>().AddForce(Vector3.up * accumulatedJumpForce, ForceMode.Impulse);
                // Resetta la forza accumulata del salto
                accumulatedJumpForce = 0f;
                // Imposta il flag del salto a falso per evitare balzi multipli
                isJumping = false;
            }
            else
            {
                // Se non sta balzando, applica la gravità normale
                if (transform.position.y > 1.5f)
                {
                    // Applica la gravità normale solo se la posizione Y è superiore a 4
                    GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
                }
                else
                {
                    // Se la posizione Y è inferiore o uguale a 4, non applicare la gravità
                    GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0f, GetComponent<Rigidbody>().velocity.z);
                }
            }
            
            Vector3 newPosition = transform.position;
            newPosition.y = Mathf.Max(newPosition.y, 1.5f); // Imposta la posizione Y minima a 4
            transform.position = newPosition;
		}
	}
    bool wasScorrimentoActive = false;

	void Update()
	{
		if (!dead)
		{   

             
			// move camera
			if (gamecam)
            {   

                
                if (Input.GetKeyDown(KeyCode.S)  && !scorrimento)
                {
                    // Controlla se la posizione y della telecamera è vicina a -5f
                    if (Mathf.Approximately(camPosition.y, -5f))
                    {     

                        originalCamPosition = camPosition; 
                        originalCamRotation = camRotation;

 
                        
                    
                        direzione = false;   
                        StartCoroutine(MoveCamera(5f, 0f, 1.5f)); // Muove la telecamera con animazione movecamera
                        
                    }
                    // Controlla se la posizione y della telecamera è vicina a 5f
                    else if (Mathf.Approximately(camPosition.y, 5f))
                    {    

                        originalCamPosition = camPosition; 
                        originalCamRotation = camRotation;

 
                        
                    
                        direzione = true;
                        StartCoroutine(MoveCamera(-5f, -180f, 1.5f)); // Muove la telecamera con animazione movecamera 
                        } 
             }
                if (scorrimento){

                    


                    StartCoroutine(MoveCameraForScorrimento(3f, 0f, -9f, 0f, 90f, 0f, 1.5f));  // StartCoroutin si usa per avviare una funzione in un thread separato
                    wasScorrimentoActive = true;


                }
                else if (wasScorrimentoActive)
                {   

                    Debug.Log("Sono entrato nell'else if" );
                    StartCoroutine(MoveCameraToOriginalPosition(originalCamPosition, originalCamRotation, 1.5f)); // Muove la telecamera con animazione movecamera
                    wasScorrimentoActive = false;
                }

                if (!scorrimento && !wasScorrimentoActive){
                    
                    


                   

                    Debug.Log("Original Cam Position: " + originalCamPosition);
                    Debug.Log("Original Cam Rotation: " + originalCamRotation);


                }
            


                gamecam.transform.position = transform.position + new Vector3(camPosition.z, camPosition.x, -camPosition.y);
                gamecam.transform.position = transform.position + new Vector3(camPosition.z, camPosition.x, -camPosition.y);

                // Modifica della rotazione della telecamera
                gamecam.transform.rotation = Quaternion.Euler(camRotation);  
				         
             }
			// attack
			if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Tab) && !isAttacking)
			{
				isAttacking = true;
				animator.SetTrigger("Attack");
				StartCoroutine(stopAttack(1));
                tryDamageTarget();


            }
            // get Hit
            if (Input.GetKeyDown(KeyCode.N) && !isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Hit");
                StartCoroutine(stopAttack(1));
            }

            animator.SetBool("isAttacking", isAttacking);

			//switch character

			if (Input.GetKeyDown("left"))
			{
				setCharacter(-1);
				isAttacking = true;
				StartCoroutine(stopAttack(1f));
			}

			if (Input.GetKeyDown("right"))
			{
				setCharacter(1);
				isAttacking = true;
				StartCoroutine(stopAttack(1f));
			}

			// death
			if (Input.GetKeyDown("m"))
				StartCoroutine(selfdestruct());

            //Leave
            if (Input.GetKeyDown("l"))
            {
                if (this.ContainsParam(animator,"Leave"))
                {
                    animator.SetTrigger("Leave");
                    StartCoroutine(stopAttack(1f));
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {   

            if (energy >= 1)
            {
            // Incrementa la forza accumulata del salto
                accumulatedJumpForce += jumpForce + (additionalJumpForce * accumulatedJumpForce);
            }
            // Imposta il flag del salto a true
            isJumping = true;

            energy -= 15;
            if (energy < 0)
            {
                energy = 0;
            }
            energyBar.value = energy;

            
                
            if (!timerStarted)
            {
                StartCoroutine(RechargeEnergy());
                timerStarted = true;
            }
            

            }

            
        }

         
	}
    GameObject target = null;
    public void tryDamageTarget()
    {
        target = null;
        float targetDistance = minAttackDistance + 1;
        foreach (var item in targets)
        {
            float itemDistance = (item.transform.position - transform.position).magnitude;
            if (itemDistance < minAttackDistance)
            {
                if (target == null) {
                    target = item;
                    targetDistance = itemDistance;
                }
                else if (itemDistance < targetDistance)
                {
                    target = item;
                    targetDistance = itemDistance;
                }
            }
        }
        if(target != null)
        {
            transform.LookAt(target.transform);
            
        }
    }
    public void DealDamage(DealDamageComponent comp)
    {
        if (target != null)
        {
            target.GetComponent<Animator>().SetTrigger("Hit");
            var hitFX = Instantiate<GameObject>(comp.hitFX);
            hitFX.transform.position = target.transform.position + new Vector3(0, target.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.y,0);
        }
    }

    public IEnumerator stopAttack(float length)
	{
		yield return new WaitForSeconds(length); 
		isAttacking = false;
	}

    public IEnumerator selfdestruct()
    {
        animator.SetTrigger("isDead");
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        dead = true;

        yield return new WaitForSeconds(3f);
        while (true)
        {
            if (Input.anyKeyDown)
            {
                Application.LoadLevel(Application.loadedLevelName);
                yield break;
            }
            else
                yield return 0;

        }
    }
    public void setCharacter(int i)
	{
		currentChar += i;

		if (currentChar > characters.Length - 1)
			currentChar = 0;
		if (currentChar < 0)
			currentChar = characters.Length - 1;

		foreach (GameObject child in characters)
		{
            if (child == characters[currentChar])
            {
                child.SetActive(true);
                if (nameText != null)
                    nameText.text = child.name;
            }
            else
            {
                child.SetActive(false);
            }
		}
		animator = GetComponentInChildren<Animator>();
    }

    public bool ContainsParam(Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) return true;
        }
        return false;
    }

    private IEnumerator MoveCamera(float targetPosY, float targetRotY, float duration)
{
    float elapsed = 0f;
    Vector3 startPos = camPosition;
    Vector3 startRot = camRotation;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Utilizza SmoothStep per rendere la transizione più fluida
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        // Interpolazione lineare della posizione e della rotazione della telecamera
        camPosition.y = Mathf.Lerp(startPos.y, targetPosY, smoothT);
        camRotation.y = Mathf.Lerp(startRot.y, targetRotY, smoothT);

        // Aggiorna la posizione e la rotazione della telecamera
        gamecam.transform.position = transform.position + new Vector3(camPosition.z, camPosition.x, -camPosition.y);
        gamecam.transform.rotation = Quaternion.Euler(camRotation);

        yield return null;
    }
}

private IEnumerator MoveCameraForScorrimento(float targetPosX, float targetPosY, float targetPosZ, float targetRotX, float targetRotY, float targetRotZ, float duration)
{
    float elapsed = 0f;
    Vector3 startPos = camPosition;
    Vector3 startRot = camRotation;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Utilizza SmoothStep per rendere la transizione più fluida
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        // Interpolazione lineare della posizione e della rotazione della telecamera
        camPosition = Vector3.Lerp(startPos, new Vector3(targetPosX, targetPosY, targetPosZ), smoothT);
        camRotation = Vector3.Lerp(startRot, new Vector3(targetRotX, targetRotY, targetRotZ), smoothT);

        // Aggiorna la posizione e la rotazione della telecamera
        gamecam.transform.position = transform.position + new Vector3(camPosition.z, camPosition.x, -camPosition.y);
        gamecam.transform.rotation = Quaternion.Euler(camRotation);

        yield return null;
    }
}

private IEnumerator MoveCameraToOriginalPosition(Vector3 targetPos, Vector3 targetRot, float duration)
{
    float elapsed = 0f;
    Vector3 startPos = gamecam.transform.position;
    Vector3 startRot = gamecam.transform.eulerAngles;

    Debug.Log("SONO IN MOVECAMERATOORIGINALPOSITION");

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Utilizza SmoothStep per rendere la transizione più fluida
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        camPosition = Vector3.Lerp(startPos, new Vector3(targetPos.x, targetPos.y, targetPos.z), smoothT);
        camRotation = Vector3.Lerp(startRot, targetRot, smoothT);

        // Interpolazione lineare della posizione e della rotazione della telecamera
        gamecam.transform.position = Vector3.Lerp(startPos, targetPos, smoothT);
        gamecam.transform.eulerAngles = Vector3.Lerp(startRot, targetRot, smoothT);

        yield return null;
    }
}
    IEnumerator RechargeEnergy()
{
    yield return new WaitForSeconds(3f); // Aspetta 1 secondo

    // Incrementa gradualmente l'energia fino a 100 solo se non si preme spazio
    while (energy < 100 )
    {   
        if (Input.GetKey(KeyCode.Space)){
            yield return RechargeEnergy(); 
        
         
        }
        energy += 1;
        energyBar.value = energy;
        
        yield return new WaitForSeconds(0.1f); // Aspetta 0.1 secondi
    }
    

    // Resetta il timer
    timerStarted = false;
}
}
