using Signals;
using Zenject;
using UnityEngine;

namespace Inputs
{
    public class DesktopInput : ITickable
    {
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.W))
                _signalBus.Fire(new AccelerationSignal { Pressed = true });

            if (Input.GetKeyUp(KeyCode.W))
                _signalBus.Fire(new AccelerationSignal { Pressed = false });

            if (Input.GetKey(KeyCode.W))
                _signalBus.Fire(new AccelerationSignal {Pressed = true});

            if (Input.GetKey(KeyCode.A))
            {
                _signalBus.Fire(new TurnSignal(){TurnIndex = 1});
            }

            if (Input.GetKey(KeyCode.D))
            {
                _signalBus.Fire(new TurnSignal(){TurnIndex = -1});
            }
            
            if(Input.GetMouseButtonDown(0)) 
                _signalBus.Fire(new BulletShootSignal());
            
            if (Input.GetMouseButton(1))
                _signalBus.Fire(new LaserShootSignal());
        }
    }
}