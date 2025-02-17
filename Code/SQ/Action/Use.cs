namespace Sandbox.SQ.Action;

public class Use : Component {
	public virtual bool Invoke ( ItemStack stack, GameObject target ) {
		return false;
	}
}
