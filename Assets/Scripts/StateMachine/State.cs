using UnityEngine;

namespace TinyProject.StateMachine
{
	/// <summary>
	/// Classe de base pour les états dans la state machine.
	/// </summary>
	public abstract class State
	{
		protected readonly MonoBehaviour Owner;
		protected Animator Animator;

		protected State(MonoBehaviour owner)
		{
			Owner = owner;
		}

		/// <summary>
		/// Initialise l'état avec les composants nécessaires.
		/// </summary>
		/// <param name="animator">L'Animator du propriétaire de l'état.</param>
		public virtual void Init(Animator animator)
		{
			Animator = animator;
		}

		/// <summary>
		/// Appelé une fois lorsque l'état est activé. 
		/// Utilisez cette méthode pour initialiser les variables ou effectuer des actions spécifiques à l'état.
		/// </summary>
		public virtual void Enter() {}

		/// <summary>
		/// Appelé une fois lorsque l'état est désactivé.
		/// Utilisez cette méthode pour nettoyer ou réinitialiser les variables avant de quitter l'état
		/// </summary>
		public virtual void Exit() {}

		/// <summary>
		/// Appelé à chaque frame tant que l'état est actif.
		/// Utilisez cette méthode pour mettre à jour les comportements ou les actions spécifiques à l'état.
		/// </summary>
		public virtual void Tick() {}

		/// <summary>
		/// Appelé à chaque frame fixe tant que l'état est actif.
		/// Utilisez cette méthode pour gérer les mises à jour physiques ou les interactions avec le moteur
		/// </summary>
		public virtual void FixedTick() {}
	}
}