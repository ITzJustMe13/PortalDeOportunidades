import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/State/ChooseImpulseState.dart';
import 'package:provider/provider.dart';

class ImpulseChooseScreen extends StatelessWidget {
  final Opportunity opportunity;
  const ImpulseChooseScreen({super.key, required this.opportunity});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          return Consumer<ChooseImpulseState>(
            builder: (context, chooseImpulseState, child) {
              return Center(
                child: Wrap(
                  children: [
                    _buildImpulseCard(
                        7, 15.99, opportunity.opportunityId, chooseImpulseState),
                    SizedBox(height: 20),
                    _buildImpulseCard(
                        30, 25.99, opportunity.opportunityId, chooseImpulseState),
                    SizedBox(height: 20),
                    _buildImpulseCard(
                        60, 55.99, opportunity.opportunityId, chooseImpulseState),
                  ],
                ),
              );
            },
          );
        },
      ),
    );
  }

  Widget _buildImpulseCard(
      int days, double value, int opportunityId, ChooseImpulseState state) {
    return ConstrainedBox(
      constraints: BoxConstraints(maxWidth: 200),
      child: Card(
        child: ListTile(
          title: Text('Impulso de $days dias'),
          subtitle: Text('Valor: $value €'),
          onTap: () async {
            await state.impulse(days, value, opportunityId);
          },
        ),
      ),
    );
  }

}
