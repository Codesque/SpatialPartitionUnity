using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Unit<T1 , T2> where T1 : MonoBehaviour where T2 : MonoBehaviour { 
    public LinkedList<T1> UnitEntity1List;    
    public LinkedList<T2> UnitEntity2List;

}




public class UnitGrid : MonoBehaviour
{

    Unit<EnemyUnit , BulletUnit>[,] cell;

    PlayerUnit player;
    Camera cam;

    public int NUM_OF_CELLS = 10; 
    public float CELL_SIZE = 1.0f;
    public float EnemySpeed = 10f;
    public float BulletSpeed = 10f;
    public GameObject EnemyPrefab;
    public BulletUnit BulletPrefab;
    public int num_of_enemies = 0;
    [Range(0f, 1f)] public float IntervalPerSpawn = 0.001f;
    public float AXIS_OFFSETT { get { return NUM_OF_CELLS * CELL_SIZE / 2; } }


    public void InitializeUnitGrid() {

        cell = new Unit<EnemyUnit, BulletUnit>[NUM_OF_CELLS, NUM_OF_CELLS];
        
        for (int i = 0; i < NUM_OF_CELLS; i++)
            for (int j = 0; j < NUM_OF_CELLS; j++) {

                cell[i, j].UnitEntity1List = new LinkedList<EnemyUnit>();
                cell[i, j].UnitEntity2List = new LinkedList<BulletUnit>();
            }
        
    }


    public int[] GetSpatialIDX(float x , float y) {

        int idx = Mathf.FloorToInt(Mathf.Clamp((x + AXIS_OFFSETT)/CELL_SIZE , 0 , NUM_OF_CELLS-1));
        int idy = Mathf.FloorToInt(Mathf.Clamp((y + AXIS_OFFSETT)/CELL_SIZE , 0 , NUM_OF_CELLS-1));


        return new int[]{idx, idy};
    
    
    }



    public void AddUnitToGrid<T>(T unit , int idx , int idy) where T : MonoBehaviour
    {
        switch (unit) {

            case EnemyUnit u: 
                cell[idx, idy].UnitEntity1List.AddLast(u); break;

            case BulletUnit b:
                cell[idx , idy].UnitEntity2List.AddLast(b); break;

            default:
                Debug.LogError(string.Format("the object named {0} is not castable to Enemy or Bullet", unit.gameObject.name));
                break;
        
        }
    
    
    }



    public void MoveUnit<T>(T unit , Vector3 newPos) where T : MonoBehaviour {

        //newPos += Vector3.one * Time.deltaTime;
        Vector3 pos = unit.transform.position;
        int[] oldIdx = GetSpatialIDX(pos.x , pos.y);
        int[] newIdx = GetSpatialIDX(newPos.x, newPos.y);


        if (oldIdx[0] == newIdx[0] && oldIdx[1] == newIdx[1]) {
            
            unit.transform.position = newPos;
            return;
        
        }

        RemoveUnitFromGrid(unit , oldIdx[0] , oldIdx[1]);
        unit.transform.position = newPos;
        AddUnitToGrid(unit , newIdx[0] , newIdx[1]);


    }


    public void RemoveUnitFromGrid<T>(T unit , int idx , int idy) where T : MonoBehaviour {

        switch (unit) { 
        
            case EnemyUnit enemyUnit:
                cell[idx,idy].UnitEntity1List.Remove(enemyUnit); 
                break;

            case BulletUnit bulletUnit:
                cell[idx,idy].UnitEntity2List.Remove(bulletUnit);
                break;

            default:
                Debug.LogError(string.Format("the object named {0} is not Enemy or Bullet so you cant remove it",unit.gameObject.name));
                break;
        
        }
    
    
    
    }








    // Start is called before the first frame update
    void Awake()
    {
        InitializeUnitGrid();
        player = GameObject.FindFirstObjectByType<PlayerUnit>();
        cam = Camera.main;
        
    }

    public void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }



    IEnumerator SpawnCoroutine() {

        while (true) {

            yield return new WaitForSeconds(IntervalPerSpawn);
            GameObject enemy = Instantiate(EnemyPrefab, new Vector3(UnityEngine.Random.Range(-AXIS_OFFSETT, AXIS_OFFSETT), UnityEngine.Random.Range(-AXIS_OFFSETT, AXIS_OFFSETT), 0f), Quaternion.identity);
            EnemyUnit e = enemy.GetComponent<EnemyUnit>();
            int[] id = new int[2];
            id = GetSpatialIDX(e.transform.position.x, e.transform.position.y);
            AddUnitToGrid(e, id[0], id[1]);
            num_of_enemies++;
        
        }
    
    
    
    }


    public BulletUnit SpawnBullet(Vector3 playerPos , Vector3 direction) {

        BulletUnit bullet = Instantiate<BulletUnit>(BulletPrefab,  playerPos, Quaternion.identity , null); 
        bullet.Direction = direction;
        bullet.Speed = BulletSpeed;

        int[] id = GetSpatialIDX(bullet.transform.position.x, bullet.transform.position.y);
        AddUnitToGrid<BulletUnit>(bullet, id[0], id[1]);
        return bullet;

    
    }



    public void KillUnit<T>(T unit) where T : MonoBehaviour
    {
        int[] id = GetSpatialIDX(unit.transform.position.x , unit.transform.position.y);
        RemoveUnitFromGrid(unit, id[0], id[1]); 
        Destroy(unit.gameObject);
        
    
    }


    public void EnemyLogic(int cellX , int cellY) {

        LinkedListNode<EnemyUnit> node = cell[cellX, cellY].UnitEntity1List.First;
        while (node != null)
        {
            EnemyUnit enemy = node.Value;
            Vector3 nextMove = Vector3.MoveTowards(enemy.transform.position, player.transform.position, EnemySpeed * Time.deltaTime);
            MoveUnit(enemy, nextMove);
            node = node.Next;


        }



    }




    void HandleCollisionUnit(BulletUnit bullet , int targetCellX , int targetCellY) {

        LinkedListNode<EnemyUnit> eNode = cell[targetCellX , targetCellY].UnitEntity1List.First;
        List<EnemyUnit> unitsToKill = new List<EnemyUnit>();
        while (eNode != null) { 
        
            EnemyUnit eUnit = eNode.Value;
            if ((eUnit.transform.position - bullet.transform.position).sqrMagnitude < 4f) {
                for (int i = 0; i < 8; i++) { 
                    BulletUnit newBull = SpawnBullet(eUnit.transform.position, new Vector3(Mathf.Sin( i * 45), Mathf.Cos(i * 45), 0f));
                    newBull.LifeTime = bullet.LifeTime * 2f;
                }

                KillUnit(eUnit);
                KillUnit(bullet);

            }

            eNode = eNode.Next;
        
        
        }
    
    
    
    }




    void HandleCollisionCell(int cellX, int cellY)
    {
        LinkedListNode<BulletUnit> bNode = cell[cellX , cellY].UnitEntity2List.First;

        while (bNode != null) { 
        

            BulletUnit bullet = bNode.Value;

            HandleCollisionUnit(bullet, cellX , cellY);

            if (cellY > 0)
                HandleCollisionUnit(bullet, cellX, cellY-1);

            if (cellX > 0 && cellY > 0)
                HandleCollisionUnit(bullet, cellX-1, cellY-1);

            if (cellX > 0)
                HandleCollisionUnit(bullet, cellX-1, cellY);

            if (cellX > 0 && cellY < NUM_OF_CELLS-1)
                HandleCollisionUnit(bullet, cellX-1, cellY + 1);
            




            bNode = bNode.Next;


        }
    
    }




    public void BulletMovementLogic(int cellX , int cellY) { 
    
    
        LinkedListNode<BulletUnit> node = cell[cellX, cellY].UnitEntity2List.First;
        while(node != null) {

            BulletUnit bullet = node.Value;
            Vector3 movVec = (bullet.Direction * BulletSpeed * Time.deltaTime); 
            Vector3 target = new Vector3 (bullet.transform.position.x + movVec.x , bullet.transform.position.y + movVec.y , bullet.transform.position.z );
            MoveUnit(bullet, target);
            node = node.Next;
        
        }
    
    
    
    }


    public void BulletSpawnLogic() {

        if (Input.GetMouseButtonDown(0)) { 
        
            Vector3 point = cam.ScreenToWorldPoint(Input.mousePosition);
            SpawnBullet(player.transform.position , (point - player.transform.position).normalized);
        
        
        }
    
    
    }


    public void BulletLifetimeLogic(int cellX , int cellY) {

        LinkedListNode<BulletUnit> node = cell[cellX, cellY].UnitEntity2List.First;
        List<BulletUnit> unitsToKill = new List<BulletUnit>();
        while (node != null)
        {

            BulletUnit bullet = node.Value;
            bullet.LifeTime += Time.deltaTime;
            if (bullet.LifeTime > 2f) unitsToKill.Add(bullet);
            node = node.Next;

        }

        for (int i = 0; i < unitsToKill.Count; i++)
            KillUnit(unitsToKill[i]);



    }



    // Update is called once per frame
    void Update()
    {

        BulletSpawnLogic();

        // Logic within this for loop changes the idx , idy assigned for the cell.
        for (int i = 0; i < NUM_OF_CELLS; i++)
            for (int j = 0; j < NUM_OF_CELLS; j++)
                EnemyLogic(i, j);


        // Logic within this for loop changes the idx , idy assigned for the cell.
        for (int i = 0; i < NUM_OF_CELLS; i++)
            for (int j = 0; j < NUM_OF_CELLS; j++)
                BulletMovementLogic(i, j);



        // Logic within this for loop wont change any cell index.
        for (int i = 0; i < NUM_OF_CELLS; i++)
            for (int j = 0; j < NUM_OF_CELLS; j++)  
                BulletLifetimeLogic(i,j);

        for (int i = 0; i < NUM_OF_CELLS; i++)
            for (int j = 0; j < NUM_OF_CELLS; j++)
                HandleCollisionCell(i,j);


            
            




    }

    
    private void OnDrawGizmos()
    {
        Vector2 start = Vector2.zero + new Vector2(-AXIS_OFFSETT ,-AXIS_OFFSETT ); 
        Vector2 end1 =  start + new Vector2(NUM_OF_CELLS * CELL_SIZE , 0f);
        Vector2 end2 = start + new Vector2(0f , NUM_OF_CELLS * CELL_SIZE );
        Vector2 end3 = start + new Vector2(NUM_OF_CELLS * CELL_SIZE, NUM_OF_CELLS * CELL_SIZE);


        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end1); 
        Gizmos.DrawLine(start, end2);
        Gizmos.DrawLine(end1, end3);
        Gizmos.DrawLine (end2, end3);


    }



}
