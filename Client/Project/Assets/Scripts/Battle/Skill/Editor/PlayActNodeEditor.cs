using UnityEngine;
using UnityEngine.UIElements;
using GraphProcessor;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System.Collections.Generic;

[NodeCustomEditor(typeof(PlayActNode))]
public class PlayActNodeView : BaseNodeView
{
    PlayActNode actNode;

    ObjectField objField;
    Button clickBtn;
    Label resText;
    public override void Enable()
    {
        base.Enable();
        // Remove useless elements
        actNode = nodeTarget as PlayActNode;

        resText = new Label("资源表名:");
        resText.text = $"资源表名: {actNode.resId}";
        controlsContainer.Add(resText);

        var asset = GetActAsset();

        objField = new ObjectField
        {
            objectType = typeof(Act.ActAsset),
            allowSceneObjects = false,
            value = asset,
        };
        objField.RegisterValueChangedCallback(ChangeActAsset);

        controlsContainer.Add(objField);

        clickBtn = new Button(RefreshData);
        if (!actNode.Equals(asset))
        {
            clickBtn.text = "读取Act数据 (有更新)";
            clickBtn.style.borderBottomColor = new Color(255, 195, 93, 255) / 255;
        }
        else
            clickBtn.text = "读取Act数据";
        controlsContainer.Add(clickBtn);

        Button btn = new Button(OpenActEditor);
        btn.text = "打开Act编辑器";
        controlsContainer.Add(btn);
    }

    bool IsChanged()
    {
        var actAsset = GetActAsset();
        return !actNode.Equals(actAsset);
    }

    void UpdatePreviewImage(Image image, UnityEngine.Object obj)
    {
        image.image = AssetPreview.GetAssetPreview(obj) ?? AssetPreview.GetMiniThumbnail(obj);
    }

    void ChangeActAsset(ChangeEvent<UnityEngine.Object> obj)
    {
        var asset = obj.newValue;
        if (asset == null)
            return;

        string assetId = asset.name;

        actNode.resId = assetId;
        resText.text = $"资源表名: {actNode.resId}";
        RefreshData();
        controlsContainer.MarkDirtyRepaint();
    }

    void RefreshData()
    {
        var actAsset = GetActAsset();
        CheckDeletedEvent(actAsset);

        actNode.SetData(actAsset);
        actNode.UpdateAllPortsLocal();
        objField.value = actAsset;

        clickBtn.text = "读取Act数据";
        clickBtn.style.borderBottomColor = Color.gray;

        outputContainer.MarkDirtyRepaint();
        this.MarkDirtyRepaint();

    }

    public bool CheckDeletedEvent(Act.ActAsset actAsset)
    {
        if (actAsset == null || actNode.actEvents == null)
            return false;

        HashSet<string> nodeEvents = new HashSet<string>(actNode.actEvents.Select(d => d.id));
        HashSet<string> events = new HashSet<string>(actAsset.Events.Select(d => d.GUID));
        nodeEvents.ExceptWith(events);
        if (nodeEvents.Count > 0)
        {
            foreach (var id in nodeEvents)
            {
                NodePort port = actNode.outputPorts.FirstOrDefault(d => d.portData.identifier == id);
                var edges = port.GetEdges();
                if (edges != null)
                {
                    foreach (var edge in new List<SerializableEdge>(edges))
                    {
                        owner.graph.Disconnect(edge.GUID);
                    }
                }
                actNode.outputPorts.Remove(port);
            }
        }
        return nodeEvents.Count > 0;
    }

    void OpenActEditor()
    {
        var asset = GetActAsset();
        Act.ActUtility.OpenTimeline(asset);
    }

    Act.ActAsset GetActAsset()
    {
        if (string.IsNullOrEmpty(actNode.resId))
            return null;


        return Act.ActUtility.GetAsset<Act.ActAsset>(actNode.resId, "ActAsset");
    }


}