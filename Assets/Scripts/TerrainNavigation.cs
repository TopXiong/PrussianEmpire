using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainNavigation : MonoBehaviour
{
    public static TerrainNavigation instance;//单例

    public class Point
    {
        public int r, c;
        public Point lastPoint;
        public Point(int r, int c, Point point)
        {
            this.r = r;
            this.c = c;
            this.lastPoint = point;
        }
        
        public Point(int r, int c)
        {
            this.r = r;
            this.c = c;
            lastPoint = this;
        }
    }
    public float ceiSideLength = 0;         // 最小格边长

    public int row;                         // 行数
    public int column;                      // 列数

    public int[] terrain;                  // 地形

    private int[,] terrains;
    private void Reset()
    {
        terrain = new int[row * column];
    }
    private void Awake()
    {
        instance = this;
        terrains = new int[row, column];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                terrains[i, j] = terrain[i * column + j];
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < row; ++i)
        {
            string str = "";
            for (int j = 0; j < column; ++j)
                str = str + terrain[i * column + j];
            //Debug.Log(str);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // 画行
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(column * ceiSideLength, 0, 0));
        for (int i = 1; i <= row; ++i)
        {
            Vector3 pos = transform.position;
            pos.y += i * ceiSideLength;
            Gizmos.DrawLine(pos, pos + new Vector3(column * ceiSideLength, 0, 0));
        }

        // 画列
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, row * ceiSideLength, 0));
        for (int i = 1; i <= column; ++i)
        {
            Vector3 pos = transform.position;
            pos.x += i * ceiSideLength;
            Gizmos.DrawLine(pos, pos + new Vector3(0, row * ceiSideLength, 0));
        }

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
                if (terrain[r * column + c] == 1)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(transform.position + new Vector3(c * ceiSideLength + ceiSideLength / 2, r * ceiSideLength + ceiSideLength / 2, 0),
                        new Vector3(ceiSideLength, ceiSideLength, 0.01f));
                }
                else
                {
                    Gizmos.color = new Color(1, 1, 1, 0);
                    Gizmos.DrawCube(transform.position + new Vector3(c * ceiSideLength + ceiSideLength / 2, r * ceiSideLength + ceiSideLength / 2, 0),
                        new Vector3(ceiSideLength, ceiSideLength, 0.01f));
                }
        }
    }


    public void SetTerrainFromPostion(Vector2 position)
    {
        float x = position.x - transform.position.x;
        float y = position.y - transform.position.y;
        if (x < 0 || y < 0) return;
        var r = (int)(y / ceiSideLength);
        var c = (int)(x / ceiSideLength);
        Debug.Log(r + "," + c);
        if (r < 0 || c < 0 || r >= row || c >= column)
            return;
        if (terrain[r * column + c] == 0)
        {
            terrain[r * column + c] = 1;
        }
        else
        {
            terrain[r * column + c] = 0;
        }

    }

    private int[,] step = {
        {1, 1},
        {1, -1},
        {-1, 1},
        {-1, -1},
        {0, 1},
        {0, -1},
        {1, 0},
        {-1, 0}
    };

    //private bool isFirst = true;
    private Stack<Point> Bfs(int r, int c, int targetR, int targetC)
    {
        bool[,] flags = new bool[row, column];
        Queue<Point> qu = new Queue<Point>();
        qu.Enqueue(new Point(r, c));
        int count = 0;
        flags[r, c] = true;
        while (qu.Count > 0)
        {
            var front = qu.First();
            if (front.r == targetR && front.c == targetC)
            {
                Stack<Point> stack = new Stack<Point>();
                if (front.lastPoint == front)
                {
                    stack.Push(front);
                    return stack;
                }
                else
                {
                    while (front.lastPoint != front)
                    {
                        stack.Push(front);
                        if (front.lastPoint.lastPoint == front.lastPoint)
                        {
                            return stack;
                        }
                        front = front.lastPoint;
                        //Debug.Log("Find");
                    }
                }
            }
            //bool hasObstacle = false;
            List<Point> li = new List<Point>();
            for (int i = 0; i < 8; ++i)
            {
                int tr = front.r + step[i, 0];
                int tc = front.c + step[i, 1];
                if (tr < 0 || tc < 0 || tr >= row || tc >= column || terrains[tr, tc] == 1 || flags[tr, tc])
                    continue;
                
                li.Add(new Point(tr, tc, front));
                flags[tr, tc] = true;
                //if (hasObstacle) continue;
                //int max = tr, min = targetR;
                //if (max < min)
                //{
                //    int t = max;
                //    max = min;
                //    min = t;
                //}
                //for (int j = min; j < max; ++j)
                //{
                //    if (terrains[j, tc] == 1)
                //    {
                //        hasObstacle = true;
                //        break;
                //    }
                //}
                //max = tc;
                //min = targetC;
                //if (max < min)
                //{
                //    int t = max;
                //    max = min;
                //    min = t;
                //}
                //for (int j = min; j < max; ++j)
                //    if (terrains[tr, j] == 1)
                //    {
                //        hasObstacle = true;
                //        break;
                //    }
            }
            li.Sort((a, b) =>
            {
                int ra = a.r - targetR;
                int ca = a.c - targetC;
                int rb = b.r - targetR;
                int cb = b.c - targetC;

                if (ra * ra + ca * ca < rb * rb + cb * cb)
                    return -1;
                else return 0;
            });
            foreach (var p in li)
            {
                //if (isFirst)
                //{
                //    int ra = p.r - targetR;
                //    int rc = p.c - targetC;
                //    Debug.Log(ra * ra + rc * rc);
                //}
                qu.Enqueue(p);
            }
            //isFirst = false;
            qu.Dequeue();
            ++count;
            if (count > 1000)
            {
                Debug.Log("BFS 执行次数大于1000次");
                var stack = new Stack<Point>();
                stack.Push(new Point(r, c));
                return stack;
            }
        }
        Debug.Log("Cant find:" + targetR + "," + targetC);
        var st = new Stack<Point>();
        st.Push(new Point(r, c));
        return st;
    }

    public Vector3 GetNextStep(Vector3 current, Vector3 target, Vector3 lastTarget, ref Stack<Point> stack)
    {
        var position = transform.position;
        int c = (int) ((current.x - position.x) / ceiSideLength);
        int r = (int) ((current.y - position.y) / ceiSideLength);
        int tc = (int)((target.x - position.x) / ceiSideLength);
        int tr = (int)((target.y - position.y) / ceiSideLength);
        //int lastc = (int)((lastPosition.x - position.x) / ceiSideLength);
        //int lastr = (int)((lastPosition.y - position.y) / ceiSideLength);
        int lasttc = (int)((lastTarget.x - position.x) / ceiSideLength);
        int lasttr = (int)((lastTarget.y - position.y) / ceiSideLength);
        if (stack == null || lasttc != c || lasttr != tr || stack.Count == 0)
        {
            stack = Bfs(r, c, tr, tc);
            System.GC.Collect();        // 调用BFS算法产生了很多垃圾，手动清理一次
        }
        //Debug.Log(c + "," + r);
        var point = stack.Pop();
        var result = new Vector3(point.c * ceiSideLength + position.x + ceiSideLength / 2,
            point.r * ceiSideLength + position.y + ceiSideLength / 2, current.z);
        if (tc == point.c)
            result.x = target.x;
        if (tr == point.r)
            result.y = target.y;

        //Debug.Log("Navigation");
        return result;
    }
}
