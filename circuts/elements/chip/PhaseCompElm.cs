using System;
using System.Collections;
using System.Collections.Generic;

namespace Circuts {

	public class PhaseCompElm : ChipElm {
		
		public PhaseCompElm(int xx, int yy, CirSim s) : base(xx, yy, s) {

		}

		public override String getChipName() {
			return "phase comparator";
		}

		public override void setupPins() {
			sizeX = 2;
			sizeY = 2;
			pins = new Pin[3];
			pins[0] = new Pin(0, SIDE_W, "I1", this);
			pins[1] = new Pin(1, SIDE_W, "I2", this);
			pins[2] = new Pin(0, SIDE_E, "O", this);
			pins[2].output = true;
		}

		public override bool nonLinear() {
			return true;
		}

		public override void stamp() {
			int vn = sim.nodeList.Count + pins[2].voltSource;
			sim.stampNonLinear(vn);
			sim.stampNonLinear(0);
			sim.stampNonLinear(nodes[2]);
		}

		bool ff1, ff2;

		public override  void doStep() {
			bool v1 = volts[0] > 2.5;
			bool v2 = volts[1] > 2.5;
			if (v1 && !pins[0].value) {
				ff1 = true;
			}
			if (v2 && !pins[1].value) {
				ff2 = true;
			}
			if (ff1 && ff2) {
				ff1 = ff2 = false;
			}
			double @out = (ff1) ? 5 : (ff2) ? 0 : -1;
			// System.out.println(out + " " + v1 + " " + v2);
			if (@out != -1) {
				sim.stampVoltageSource(0, nodes[2], pins[2].voltSource, @out);
			} else {
				// tie current through output pin to 0
				int vn = sim.nodeList.Count + pins[2].voltSource;
				sim.stampMatrix(vn, vn, 1);
			}
			pins[0].value = v1;
			pins[1].value = v2;
		}

		public override int getPostCount() {
			return 3;
		}

		public override int getVoltageSourceCount() {
			return 1;
		}
		
	}
}