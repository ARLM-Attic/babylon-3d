using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace BABYLON {
public class Condition {
public ActionManager _actionManager;
public float _evaluationId;
public bool _currentResult;
public Condition(ActionManager actionManager) {
this._actionManager=actionManager;
}
public virtual bool isValid() {
return true;
}
public virtual string _getProperty(string propertyPath) {
return this._actionManager._getProperty(propertyPath);
}
public virtual object _getEffectiveTarget(object target, string propertyPath) {
return this._actionManager._getEffectiveTarget(target, propertyPath);
}
}
public class ValueCondition: Condition {
private static dynamic _IsEqual=0;
private static dynamic _IsDifferent=1;
private static dynamic _IsGreater=2;
private static dynamic _IsLesser=3;
public ActionManager _actionManager;
private object _target;
private string _property;
public string propertyPath;
public object value;
public float operator;
public ValueCondition(ActionManager actionManager, object target, string propertyPath, object value, float operator) {
base(actionManager);
this._target=this._getEffectiveTarget(target, this.propertyPath);
this._property=this._getProperty(this.propertyPath);
}
public virtual bool isValid() {
switch (this.operator) {
case ValueCondition.IsGreater: 
return this._target[this._property]>this.value;
case ValueCondition.IsLesser: 
return this._target[this._property]<this.value;
case ValueCondition.IsEqual: 
case ValueCondition.IsDifferent: 
var check;
if (this.value.equals) 
{
check=this.value.equals(this._target[this._property]);
}
else 
{
check=this.value==this._target[this._property];
}
return (this.operator==ValueCondition.IsEqual) ? check : !check;
}
return false;
}
}
public class PredicateCondition: Condition {
public ActionManager _actionManager;
public Func<bool> predicate;
public PredicateCondition(ActionManager actionManager, Func<bool> predicate) {
base(actionManager);
}
public virtual bool isValid() {
return this.predicate();
}
}
public class StateCondition: Condition {
public ActionManager _actionManager;
private object _target;
public string value;
public StateCondition(ActionManager actionManager, object target, string value) {
base(actionManager);
this._target=target;
}
public virtual bool isValid() {
return this._target.state==this.value;
}
}
}
