using System.Collections;
using Games.Reefscape.Enums;
using Games.Reefscape.GamePieceSystem;
using Games.Reefscape.Robots;
using MoSimCore.BaseClasses.GameManagement;
using MoSimCore.Enums;
using MoSimLib;
using RobotFramework.Components;
using RobotFramework.Controllers.GamePieceSystem;
using RobotFramework.Controllers.PidSystems;
using RobotFramework.Enums;
using RobotFramework.GamePieceSystem;
using UnityEngine;

namespace Prefabs.Reefscape.Robots.Mods._4481.Remb
{
    public class Rembrantv1 : ReefscapeRobotBase
    {

        [SerializeField] private GenericElevator elevator;
        [SerializeField] private GenericJoint arm;
        // [SerializeField] private GenericJoint climbArm;
        // [SerializeField] private GenericJoint climbWrist;
        // [SerializeField] private GenericJoint droppyThing;
        // [SerializeField] private RembrandtClimber climbHitboxes;

        [Header("Pids")]
        [SerializeField] private PidConstants armPid;
        [SerializeField] private PidConstants elevatorPid;
        // [SerializeField] private PidConstants climbArmPid;
        // [SerializeField] private PidConstants climbWristPid;
        // [SerializeField] private PidConstants droppyThingPid;

        [Header("Setpoints")]
        [SerializeField] private Rembrantsetpoint stow;

        [SerializeField] private Rembrantsetpoint intake;
        [SerializeField] private Rembrantsetpoint l1;
        [SerializeField] private Rembrantsetpoint l2;
        [SerializeField] private Rembrantsetpoint l3;
        [SerializeField] private Rembrantsetpoint l4;
        [SerializeField] private Rembrantsetpoint lowAlgae;
        [SerializeField] private Rembrantsetpoint highAlgae;
        [SerializeField] private Rembrantsetpoint bargePrep;
        [SerializeField] private Rembrantsetpoint bargePlace;
        [SerializeField] private Rembrantsetpoint groundAlgae;
        [SerializeField] private Rembrantsetpoint lollipop;
        [SerializeField] private Rembrantsetpoint climb;
        [SerializeField] private Rembrantsetpoint climbed;
        [SerializeField] private Rembrantsetpoint climbRetract1;
        [SerializeField] private Rembrantsetpoint climbRetract2;

        [Header("Barge Algae Variables")]
        // [SerializeField] private float vertical;
        // [SerializeField] private float horizontal;
        // [SerializeField] private float bargeDelay;

        // [Header("Intakes")]
        [SerializeField] private ReefscapeGamePieceIntake coralIntake;
        // [SerializeField] private ReefscapeGamePieceIntake algaeIntake;

        [Header("Game Piece States")]
        [SerializeField] private GamePieceState coralStowState;
        // [SerializeField] private GamePieceState coralL4State;
        // [SerializeField] private GamePieceState algaeStowState;
        // [SerializeField] private GenericAnimationJoint[] algaeRollers;
        // [SerializeField] private GenericAnimationJoint[] topCoralRollers;
        // [SerializeField] private GenericAnimationJoint[] bottomCoralRollers;
        // [SerializeField] private GenericRoller[] rollers;
        // [SerializeField] private Collider[] EEcolliders;

        [Header("Audio")]
        // [SerializeField] private AudioSource algaeStallSource;
        // [SerializeField] private AudioClip algaeStallAudio;
        // [SerializeField] private AudioSource algaeRollerSource;
        // [SerializeField] private AudioClip algaeRollerAudio;
        // [SerializeField] private AudioSource coralRollerSource;
        // [SerializeField] private AudioClip coralRollerAudio;
        private float _algaeRollerTargetSpeed;
        private float _coralRollerTargetSpeed;

        private bool algaePlaced = false;
        private bool retractingClimb = false;
        private bool climbing = false;
        private RobotGamePieceController<ReefscapeGamePiece, ReefscapeGamePieceData>.GamePieceControllerNode _coralController;
        private RobotGamePieceController<ReefscapeGamePiece, ReefscapeGamePieceData>.GamePieceControllerNode _algaeController;




        private float _elevatorTargetHeight;
        private float _armTargetAngle;
        private float _climbArmTargetAngle;
        private float _climbWristTargetAngle;
        private float _droppyThingTargetAngle;
        private string intakeSetpoint;
        private bool _alreadyPlaced = false;
        private bool algaeSpinning = false;

        protected override void Start()
        {
            
            RobotGamePieceController.SetPreload(coralStowState);
            base.Start();
            _elevatorTargetHeight = 0;
            _armTargetAngle = 0;
            arm.SetPid(armPid);
            _coralController = RobotGamePieceController.GetPieceByName(ReefscapeGamePieceType.Coral.ToString());
            _coralController.intakes.Add(coralIntake);
            _coralController.gamePieceStates = new[]
            {
        coralStowState,
            };
        }

        private void SetSetpoint(Rembrantsetpoint setpoint)
        {
            _elevatorTargetHeight = setpoint.elevatorHeight;
            _armTargetAngle = setpoint.armAngle;
        }

        private void UpdateSetpoints()
        {
            elevator.SetTarget(_elevatorTargetHeight);
            arm.SetTargetAngle(_armTargetAngle).withAxis(JointAxis.X);
        }
        private void FixedUpdate()
        {
            _coralController.SetTargetState(coralStowState);
            switch (CurrentSetpoint)
            {
                case ReefscapeSetpoints.Stow:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.Intake:
                    SetSetpoint(intake);
                    _coralController.RequestIntake(coralIntake, true);
                    _coralController.SetTargetState(coralStowState);
                    break;
                case ReefscapeSetpoints.Place:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.L1:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.Stack:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.L2:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.LowAlgae:
                    SetSetpoint(stow);

                    break;
                case ReefscapeSetpoints.L3:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.HighAlgae:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.L4:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.Processor:
                    break;
                case ReefscapeSetpoints.Barge:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.RobotSpecial:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.Climb:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.Climbed:
                    SetSetpoint(stow);
                    break;
            }
            UpdateSetpoints();
        }
    }
}