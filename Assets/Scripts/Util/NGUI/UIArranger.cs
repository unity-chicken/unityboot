using UnityEngine;
using System.Collections;

#if NGUI_SUPPORT
public class UIArranger : MonoBehaviour {
    public enum AnchroType {
        Center,
        Left,
        Right,
        Top,
        Bottom,
        LeftTop,
        LeftBottom,
        RightTop,
        RightBottom,
    }

    public AnchroType anchor;
    public AnchroType to;
    public int leftPadding = 15;
    public int rightPadding = 15;
    public int topPadding = 15;
    public int bottomPadding = 15;
    public bool onlyActive = true;
    public bool touchBubbling = false;
    public Vector3 touchCenter = Vector3.zero;
    public Vector3 touchSize = Vector3.one;
    public string filter = "";

    public bool autoScale = false;
    private float defaultRatio = 0.625f; // 10 : 16
    
    void Start() {
        if (autoScale) {
            float ratio = (float)Screen.height / (float)Screen.width;
            float scaler = ratio / defaultRatio;
            scaler = Mathf.Max(1, scaler);
            scaler = 1f / scaler;
            transform.localScale *= scaler;
            leftPadding   = (int)(leftPadding   * scaler);
            rightPadding  = (int)(rightPadding  * scaler);
            topPadding    = (int)(topPadding    * scaler);
            bottomPadding = (int)(bottomPadding * scaler);
        }

        Arrange();
    }

    public bool IsTouchPointIn() {
        if (touchBubbling) {
            return false;
        }

        Vector3 point = Input.mousePosition;
        float scale = (float)Screen.height / (float)transform.root.GetComponent<UIRoot>().manualHeight;

        point.x -= Screen.width / 2;
        point.y -= Screen.height / 2;
        point /= scale;
        return IsPointInRect(point.x, point.y);
    }

    public bool IsPointInRect(float x, float y) {
        Rect rect = GetChildSize();
        Vector3 center = transform.localPosition + touchCenter;

        rect.xMin *= touchSize.x;
        rect.xMax *= touchSize.x;
        rect.yMin *= touchSize.y;
        rect.yMax *= touchSize.y;
        rect.xMin += center.x;
        rect.yMin += center.y;
        rect.xMax += center.x;
        rect.yMax += center.y;

        if (x < rect.xMin || rect.xMax < x) {
            return false;
        }
        if (y < rect.yMin || rect.yMax < y) {
            return false;
        }

        return true;
    }

    public void Arrange() {
        Rect rect = GetChildSize();
        Vector3 anchorPt = Vector3.zero;
        float z = transform.position.z;

        rect.xMin -= leftPadding;
        rect.xMax += rightPadding;
        rect.yMin -= bottomPadding;
        rect.yMax += topPadding;

        float centerX = rect.xMin + rect.width/2;
        float centerY = rect.yMin + rect.height/2;

        switch(anchor) {
            case AnchroType.Center:
                anchorPt = new Vector3(centerX, centerY, z);
                break;
            case AnchroType.Left:
                anchorPt = new Vector3(rect.xMin, centerY, z);
                break;
            case AnchroType.Right:
                anchorPt = new Vector3(rect.xMax, centerY, z);
                break;
            case AnchroType.Top:
                anchorPt = new Vector3(centerX, rect.yMax, z);
                break;
            case AnchroType.Bottom:
                anchorPt = new Vector3(centerX, rect.yMin, z);
                break;
            case AnchroType.LeftTop:
                anchorPt = new Vector3(rect.xMin, rect.yMax, z);
                break;
            case AnchroType.LeftBottom:
                anchorPt = new Vector3(rect.xMin, rect.yMin, z);
                break;
            case AnchroType.RightTop:
                anchorPt = new Vector3(rect.xMax, rect.yMax, z);
                break;
            case AnchroType.RightBottom:
                anchorPt = new Vector3(rect.xMax, rect.yMin, z);
                break;
        }

        UIRoot root = (UIRoot)GameObject.FindObjectOfType(typeof(UIRoot));
        float halfWidth = (Screen.width * ((float)root.activeHeight/Screen.height)) / 2;
        float halfHeight = root.activeHeight / 2;

        Vector3 position = Vector3.zero;
        switch(to) {
            case AnchroType.Left:
                position.x = -halfWidth;
                break;
            case AnchroType.Right:
                position.x = halfWidth;
                break;
            case AnchroType.Top:
                position.y = halfHeight;
                break;
            case AnchroType.Bottom:
                position.y = -halfHeight;
                break;
            case AnchroType.LeftTop:
                position.x = -halfWidth;
                position.y = halfHeight;
                break;
            case AnchroType.LeftBottom:
                position.x = -halfWidth;
                position.y = -halfHeight;
                break;
            case AnchroType.RightTop:
                position.x = halfWidth;
                position.y = halfHeight;
                break;
            case AnchroType.RightBottom:
                position.x = halfWidth;
                position.y = -halfHeight;
                break;
        }

        transform.localPosition = position  - anchorPt;
    }

    Vector3 getScale(Transform transform) {
        Vector3 scale = Vector3.one;

        Transform current = transform;
        while (current!= null) {
            scale.x *= current.localScale.x;
            scale.y *= current.localScale.y;
            scale.z *= current.localScale.z;

            if (current == this.transform) {
                break;
            }
            current = current.parent;
        }

        return scale;
    }

    Vector3 getPosition(Transform transform) {
        Vector3 position = Vector3.zero;

        Transform current = transform;
        while (current!= null) {
            if (current == this.transform) {
                break;
            }
            
            position.x += current.localPosition.x;
            position.y += current.localPosition.y;
            position.z += current.localPosition.z;
            current = current.parent;
        }

        return position;
    }

    public Rect GetChildSize() {
        Rect rect = new Rect();
        bool flagFirst = true;

        UIWidget[] widgets = GetComponentsInChildren<UIWidget>();
        foreach (UIWidget widget in widgets) {
            if (onlyActive) {
                if (widget.gameObject.activeSelf == false) {
                    continue;
                }
            }

            if (widget.color.a == 0) {
                continue;
            }

            if (filter != "") {
                if (widget.gameObject.name.IndexOf(filter) != -1) {
                    continue;
                }
            }

            Vector3 scale = getScale(widget.transform);
            Vector3 position = getPosition(widget.transform);
            float width = widget.width * scale.x;
            float height = widget.height * scale.y;

            float xMin = position.x * scale.x - width/2;
            float xMax = position.x * scale.x + width/2;
            float yMin = position.y * scale.y - height/2;
            float yMax = position.y * scale.y + height/2;

            switch (widget.pivot) {
                case UIWidget.Pivot.TopLeft:
                case UIWidget.Pivot.Top:
                case UIWidget.Pivot.TopRight:
                    yMin -= height/2;
                    yMax -= height/2;
                    break;
                case UIWidget.Pivot.BottomLeft:
                case UIWidget.Pivot.Bottom:
                case UIWidget.Pivot.BottomRight:
                    yMin += height/2;
                    yMax += height/2;
                    break;
            }

            switch (widget.pivot) {
                case UIWidget.Pivot.TopLeft:
                case UIWidget.Pivot.Left:
                case UIWidget.Pivot.BottomLeft:
                    xMin += width/2;
                    xMax += width/2;
                    break;

                case UIWidget.Pivot.TopRight:
                case UIWidget.Pivot.Right:
                case UIWidget.Pivot.BottomRight:
                    xMin -= width/2;
                    xMax -= width/2;
                    break;
            }

            if (flagFirst == true) {
                rect.xMin = xMin;
                rect.xMax = xMax;
                rect.yMin = yMin;
                rect.yMax = yMax;
                flagFirst = false;
                continue;
            }
            else {
                if (xMin < rect.xMin) {
                    rect.xMin = xMin;
                }
                if (yMin < rect.yMin) {
                    rect.yMin = yMin;
                }
                if (xMax > rect.xMax) {
                    rect.xMax = xMax;
                }
                if (yMax > rect.yMax) {
                    rect.yMax = yMax;
                }
            }
        }

        return rect;
    }
}
#endif