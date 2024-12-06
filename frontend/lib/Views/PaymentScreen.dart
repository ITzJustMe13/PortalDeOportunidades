import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/State/PaymentState.dart';
import 'package:provider/provider.dart';

class PaymentScreen extends StatefulWidget {
  final bool isSuccess;
  final String paymentType; // e.g., "Impulse" or "Reservation"
  final VoidCallback onNavigateHome;

  const PaymentScreen({
    super.key,
    required this.isSuccess,
    required this.paymentType,
    required this.onNavigateHome,
  });

  @override
  State<PaymentScreen> createState() => _PaymentScreenState();

  static Future<PaymentScreen> fromUri(
      Uri uri, VoidCallback onNavigateHome) async {
    // Determine if success or cancel page
    bool isSuccess = uri.pathSegments.contains('success');
    String paymentType = uri.queryParameters['paymentType'] ?? 'unknown';

    return PaymentScreen(
      isSuccess: isSuccess,
      paymentType: paymentType,
      onNavigateHome: onNavigateHome,
    );
  }
}

class _PaymentScreenState extends State<PaymentScreen> {
  bool _isHandled = false;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    if (!_isHandled) {
      final paymentState = Provider.of<PaymentState>(context, listen: false);
      _isHandled = true; // Mark as handled

      if (widget.isSuccess) {
        if (widget.paymentType == "reservation") {
          paymentState.handleReservation();
        } else if (widget.paymentType == "impulse") {
          paymentState.handleImpulse();
        }
      }
      if (widget.paymentType == "reservation") {
        paymentState.deleteReservation();
      } else if (widget.paymentType == "impulse") {
        paymentState.deleteImpulse();
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final paymentState = Provider.of<PaymentState>(context);
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            return _buildMobileLayout(context, paymentState);
          } else if (constraints.maxWidth < 1200) {
            return _buildTabletLayout(context, paymentState);
          } else {
            return _buildDesktopLayout(context, paymentState);
          }
        },
      ),
    );
  }

  // For mobile layout
  Widget _buildMobileLayout(BuildContext context, PaymentState paymentState) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            widget.isSuccess ? Icons.check_circle : Icons.error,
            color: widget.isSuccess ? Colors.green : Colors.red,
            size: 100,
          ),
          const SizedBox(height: 16),
          Text(
            widget.isSuccess
                ? "${widget.paymentType} Pagamento Bem-Sucedido!"
                : "${widget.paymentType} Pagamento Falhou!",
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: widget.isSuccess ? Colors.green : Colors.red,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(
            widget.isSuccess
                ? (widget.paymentType.toLowerCase() == "reservation"
                    ? "Pagamento de Reserva Bem-Sucedido"
                    : "Pagamento de Impulso Bem-Sucedido")
                : "O pagamento falhou. Por favor, tente novamente.",
            style: const TextStyle(fontSize: 16),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 24),
          if (paymentState.isLoading)
            CircularProgressIndicator()
          else
            ElevatedButton(
              onPressed: widget.onNavigateHome,
              child: Text("Voltar ao Início"),
            ),
          if (paymentState.errorMessage.isNotEmpty)
            Text(
              paymentState.errorMessage,
              style: TextStyle(color: Colors.red),
            ),
        ],
      ),
    );
  }

  // For tablet layout
  Widget _buildTabletLayout(BuildContext context, PaymentState paymentState) {
    return Center(
      child: SizedBox(
        width: 500,
        child: _buildMobileLayout(context, paymentState),
      ),
    );
  }

  // For desktop layout
  Widget _buildDesktopLayout(BuildContext context, PaymentState paymentState) {
    return Center(
      child: SizedBox(
        width: 800,
        child: Row(
          children: [
            Expanded(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      widget.isSuccess
                          ? (widget.paymentType.toLowerCase() == "reservation"
                              ? "Pagamento de Reserva Bem-Sucedido"
                              : "Pagamento de Impulso Bem-Sucedido")
                          : "O pagamento falhou. Por favor, tente novamente.",
                      style: TextStyle(
                        fontSize: 32,
                        fontWeight: FontWeight.bold,
                        color: widget.isSuccess ? Colors.green : Colors.red,
                      ),
                    ),
                    const SizedBox(height: 16),
                    Text(
                      widget.isSuccess
                          ? (widget.paymentType.toLowerCase() == "reservation"
                              ? "Pagamento de Reserva Bem-Sucedido"
                              : "Pagamento de Impulso Bem-Sucedido")
                          : "O pagamento falhou. Por favor, tente novamente.",
                      style: TextStyle(fontSize: 18),
                    ),
                    const SizedBox(height: 24),
                    if (paymentState.isLoading)
                      CircularProgressIndicator()
                    else
                      ElevatedButton(
                        onPressed: widget.onNavigateHome,
                        child: Text("Voltar ao Início"),
                      ),
                    if (paymentState.errorMessage.isNotEmpty)
                      Text(
                        paymentState.errorMessage,
                        style: TextStyle(color: Colors.red),
                      ),
                  ],
                ),
              ),
            ),
            Expanded(
              child: Icon(
                widget.isSuccess ? Icons.check_circle : Icons.error,
                color: widget.isSuccess ? Colors.green : Colors.red,
                size: 150,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
