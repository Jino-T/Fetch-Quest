using System;
using System.Collections.Generic;
using UnityEngine;

public class NewBetterHook : MonoBehaviour
{
    public GameObject ropeHingObj; // Assign this in the Inspector or via constructor.

    private Node sentinel;

    public int count = 0;

    public class Node
    {
        public string name;
        public GameObject thisObj;
        private Node nodeLeft;
        private Node nodeRight;

        public int index;

        public Node(GameObject cloneObj)
        {
            this.thisObj = cloneObj;
            this.nodeLeft = null;
            this.nodeRight = null;

            index = 0;
        }

        /*
        public Node(Node left, Node right, GameObject ropeHingObj, Vector2 collisionPoint)
        {
            if (ropeHingObj != null)
            {
                thisObj = Instantiate(ropeHingObj, collisionPoint, Quaternion.identity);
            }

            SetNodeLeft(left);
            SetNodeRight(right);

            left?.SetNodeRight(this);
            right?.SetNodeLeft(this);
        }
        */

        public Node(Node left, Node right, GameObject ropeHingObj)
        {

            thisObj = ropeHingObj;


            // Set left and right references but don't link back immediately to avoid cycles
            this.nodeLeft = left;
            this.nodeRight = right;

            left.nodeRight = this;
            right.nodeLeft =this;

            index = left.index+1;

            //Debug.Log("left: "+ this.nodeLeft);
            //Debug.Log("right: "+ this.nodeRight);
            //Debug.Log("left Right: "+ this.nodeLeft.nodeRight);
            //Debug.Log("right Left: "+ this.nodeRight.nodeLeft);


        }


        public Node GetLeftNode() => this.nodeLeft;
        public Node GetRightNode() => this.nodeRight;
        public GameObject GetObj() => this. thisObj;

        public void SetNodeLeft(Node left) => nodeLeft = left;
        public void SetNodeRight(Node right) => nodeRight = right;

        public void setObj(GameObject obj) => this.thisObj = obj;

        public void setName(string name) => this.name = name;

        public string getName(){
            return (this.name);
        }
    }


    public NewBetterHook( GameObject jointPrefab)
    {
        ropeHingObj = jointPrefab;
        sentinel = new Node(null);
        sentinel.setName("sentinel");
        
    }

    public NewBetterHook( GameObject head,GameObject jointPrefab)
    {

        ropeHingObj = jointPrefab;
        sentinel = new Node(null);

        sentinel.setName("sentinel");
        this.AddNewNode(head);
        
        
    }

     public NewBetterHook( GameObject head, GameObject tail ,GameObject jointPrefab)
    {

        ropeHingObj = jointPrefab;
        sentinel = new Node(null);

        sentinel.setName("sentinel");
        this.AddNewNode(head);
        this.AddNewNode(tail);
        
    }

    
    public NewBetterHook()
    {
        
        sentinel = new Node(null);
        sentinel.setName("sentinel");
        
    }

    private void FixedUpdate()
    {
        // Check rope status or perform other per-frame logic if necessary
    }

    public IEnumerable<Node> IterateNodes()
{
    HashSet<Node> visited = new HashSet<Node>();
    Node current = sentinel.GetRightNode();
    while (current != null && current != sentinel && !visited.Contains(current))
    {
        yield return current;
        visited.Add(current);
        current = current.GetRightNode();
    }
}


public Node AddNewNode(Node leftNode, Node rightNode, Vector2 collisionPoint)
{
    count += 1;
    if (leftNode == null || rightNode == null) return null;

    GameObject thisObj = Instantiate(ropeHingObj, collisionPoint, Quaternion.identity);
    Node newNode = new Node(leftNode, rightNode, thisObj);

    // Set position and link neighbors
    SetNodePosition(newNode, collisionPoint);
    LinkNeighbors(leftNode, newNode, rightNode);

    return newNode;
}

public Node AddNewNode(Node leftNode, Node rightNode, GameObject collisionPoint)
{
    count +=1;
    if (leftNode == null || rightNode == null || collisionPoint == null) return null;

    Node newNode = new Node(leftNode, rightNode, collisionPoint);

    // Link neighbors without setting position (using existing GameObject position)
    LinkNeighbors(leftNode, newNode, rightNode);

    return newNode;
}

public Node AddNewNode(Node leftNode, Node rightNode)
{
    count += 1;
    if (leftNode == null || rightNode == null) return null;

    Node newNode = new Node(leftNode, rightNode, ropeHingObj);

    // Link neighbors without specific position
    LinkNeighbors(leftNode, newNode, rightNode);

    return newNode;
}

public Node getSentinel(){
    return sentinel;
}

public Node AddNewNode(GameObject positionObj)
{

    count += 1;
    if (positionObj == null) return null;

    Node newNode = new Node(positionObj);

    if (sentinel.GetLeftNode() == null)
    {
        // Initialize list with a single node
        sentinel.SetNodeLeft(newNode);
        sentinel.SetNodeRight(newNode);
        newNode.SetNodeLeft(sentinel);
        newNode.SetNodeRight(sentinel);
    }
    else
    {
        /*
        Node head = sentail.GetRightNode();
        newNode.SetNodeRight(head);
        newNode.SetNodeLeft(sentinel);
        head.SetNodeLeft(newNode);
        sentail.SetNodeRight(newNode);
        */

        // Add to the end of the list
        
        Node tail = sentinel.GetLeftNode();
        newNode.SetNodeLeft(tail);
        newNode.SetNodeRight(sentinel);
        tail.SetNodeRight(newNode);
        sentinel.SetNodeLeft(newNode);
        
    }

    return newNode;
}

// Helper to set node position
private void SetNodePosition(Node node, Vector2 position)
{
    if (node.GetObj() != null)
    {
        node.GetObj().transform.position = position;
    }
}

// Helper to link neighbors
private void LinkNeighbors(Node leftNode, Node newNode, Node rightNode)
{



    leftNode.SetNodeRight(newNode);
    newNode.SetNodeLeft(leftNode);

    if (rightNode != null)
    {
        rightNode.SetNodeLeft(newNode);
        newNode.SetNodeRight(rightNode);
    }
}


public float popNode(Node newNode)
{
    count -=1;
    Node leftNode = newNode.GetLeftNode();
    Node rightNode = newNode.GetRightNode();
    float dist = 0f;

    if (leftNode != null)
    {
        leftNode.SetNodeRight(rightNode);
    }

    if (rightNode != null)
    {
        rightNode.SetNodeLeft(leftNode);
    }

    // Optional: Destroy the node's associated GameObject to clean up
    if (newNode.GetObj() != null)
    {
        DistanceJoint2D newJoint = newNode.GetObj().GetComponent<DistanceJoint2D>();
        DistanceJoint2D leftJoint = leftNode.GetObj().GetComponent<DistanceJoint2D>();
        if (newJoint != null){
            dist = newJoint.distance + leftJoint.distance ;
        }

        //Destroy(newNode.GetObj());
    }

    // Nullify the newNode's links (not strictly necessary, but for clarity)
    newNode.SetNodeLeft(null);
    newNode.SetNodeRight(null);
    return dist;
}



    public static Vector2? CheckLayerOverlap(Vector2 pointA, Vector2 pointB, int layerIndex)
    {
        LayerMask layerMask = 1 << layerIndex;
        Vector2 direction = pointB - pointA;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(pointA, direction, distance, layerMask);

        return hit.collider != null ? hit.point : (Vector2?)null;
    }

    public void PrintNodeNames()
    {
        Node curr = sentinel.GetRightNode();
        while (curr != sentinel && curr != null){
            Debug.Log(curr.GetObj().name);
            curr = curr.GetRightNode();
        }
    }

    public void testAdd()
    {
        AddNewNode(sentinel.GetRightNode(),sentinel.GetRightNode().GetRightNode());
    }


    public Node GetHead() => sentinel.GetRightNode();
    public Node GetTail() =>  sentinel.GetLeftNode();
}
