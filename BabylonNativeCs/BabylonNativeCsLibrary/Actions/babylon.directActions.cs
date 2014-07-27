using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace BABYLON {
public class SwitchBooleanAction: Action {
private object _target;
private string _property;
public string propertyPath;
public SwitchBooleanAction(object triggerOptions, object target, string propertyPath, Condition condition) {
base(triggerOptions, condition);
this._target=target;
}
public virtual void _prepare() {
this._target=this._getEffectiveTarget(this._target, this.propertyPath);
this._property=this._getProperty(this.propertyPath);
}
public virtual void execute() {
this._target[this._property]=!this._target[this._property];
}
}
public class SetStateAction: Action {
private object _target;
public string value;
public SetStateAction(object triggerOptions, object target, string value, Condition condition) {
base(triggerOptions, condition);
this._target=target;
}
public virtual void execute() {
this._target.state=this.value;
}
}
public class SetValueAction: Action {
private object _target;
private string _property;
public string propertyPath;
public object value;
public SetValueAction(object triggerOptions, object target, string propertyPath, object value, Condition condition) {
base(triggerOptions, condition);
this._target=target;
}
public virtual void _prepare() {
this._target=this._getEffectiveTarget(this._target, this.propertyPath);
this._property=this._getProperty(this.propertyPath);
}
public virtual void execute() {
this._target[this._property]=this.value;
}
}
public class IncrementValueAction: Action {
private object _target;
private string _property;
public string propertyPath;
public object value;
public IncrementValueAction(object triggerOptions, object target, string propertyPath, object value, Condition condition) {
base(triggerOptions, condition);
this._target=target;
}
public virtual void _prepare() {
this._target=this._getEffectiveTarget(this._target, this.propertyPath);
this._property=this._getProperty(this.propertyPath);
if (typeof (this._target[this._property])!="numbe") 
{
Tools.Warn("Warning: IncrementValueAction can only be used with number value");
}
}
public virtual void execute() {
this._target[this._property]+=this.value;
}
}
public class PlayAnimationAction: Action {
private object _target;
public float from;
public float to;
public bool loop;
public PlayAnimationAction(object triggerOptions, object target, float from, float to, bool loop, Condition condition) {
base(triggerOptions, condition);
this._target=target;
}
public virtual void _prepare() {
}
public virtual void execute() {
var scene = this._actionManager.getScene();
scene.beginAnimation(this._target, this.from, this.to, this.loop);
}
}
public class StopAnimationAction: Action {
private object _target;
public StopAnimationAction(object triggerOptions, object target, Condition condition) {
base(triggerOptions, condition);
this._target=target;
}
public virtual void _prepare() {
}
public virtual void execute() {
var scene = this._actionManager.getScene();
scene.stopAnimation(this._target);
}
}
public class DoNothingAction: Action {
public DoNothingAction(object triggerOptions, Condition condition) {
base(triggerOptions, condition);
}
public virtual void execute() {
}
}
public class CombineAction: Action {
public Action[] children;
public CombineAction(object triggerOptions, Action[] children, Condition condition) {
base(triggerOptions, condition);
}
public virtual void _prepare() {
for (var index = 0;index<this.children.length;index++) 
{
this.children[index]._actionManager=this._actionManager;
this.children[index]._prepare();
}
}
public virtual void execute(ActionEvent evt) {
for (var index = 0;index<this.children.length;index++) 
{
this.children[index].execute(evt);
}
}
}
public class ExecuteCodeAction: Action {
public Func<ActionEvent, object> func;
public ExecuteCodeAction(object triggerOptions, Func<ActionEvent, object> func, Condition condition) {
base(triggerOptions, condition);
}
public virtual void execute(ActionEvent evt) {
this.func(evt);
}
}
public class SetParentAction: Action {
private object _parent;
private object _target;
public SetParentAction(object triggerOptions, object target, object parent, Condition condition) {
base(triggerOptions, condition);
this._target=target;
this._parent=parent;
}
public virtual void _prepare() {
}
public virtual void execute() {
if (this._target.parent==this._parent) 
{
return;
}
var invertParentWorldMatrix = this._parent.getWorldMatrix().clone();
invertParentWorldMatrix.invert();
this._target.position=BABYLON.Vector3.TransformCoordinates(this._target.position, invertParentWorldMatrix);
this._target.parent=this._parent;
}
}
}
